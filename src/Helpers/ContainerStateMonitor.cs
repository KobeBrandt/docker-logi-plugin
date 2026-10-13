namespace Loupedeck.DockerPlugin.Helpers;

using Types;

public sealed class ContainerStateMonitor
{
    private readonly IDockerClient _client;
    private readonly Object _sync = new();

    private Dictionary<String, String> _lastStatesById = new();
    private volatile List<DockerContainer> _containers;

    public ContainerStateMonitor(IDockerClient client) =>
        this._client = client ?? throw new ArgumentNullException(nameof(client));

    public event EventHandler StatesChanged;

    public List<DockerContainer> Containers => this._containers;

    public void Refresh()
    {
        var containers = this._client.GetAllContainers().Result;
        if (this.StoreAndDetectChange(containers))
        {
            this.StatesChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    // Bounded so a slow or refused Docker connection cannot stall action loading; a late result still raises StatesChanged.
    public void RefreshIfNeverLoaded(TimeSpan maxWait)
    {
        if (this._containers != null)
        {
            return;
        }

        try
        {
            Task.Run(this.Refresh).Wait(maxWait);
        }
        catch (AggregateException ex)
        {
            PluginLog.Error(ex, "Initial container listing failed");
        }
    }

    private Boolean StoreAndDetectChange(List<DockerContainer> containers)
    {
        var statesById = ToStatesById(containers);
        lock (this._sync)
        {
            var changed = !HaveSameStates(this._lastStatesById, statesById);
            this._lastStatesById = statesById;
            this._containers = containers;
            return changed;
        }
    }

    private static Dictionary<String, String> ToStatesById(List<DockerContainer> containers) =>
        containers?.ToDictionary(container => container.Id, container => container.State) ?? new();

    private static Boolean HaveSameStates(Dictionary<String, String> previous, Dictionary<String, String> current) =>
        previous.Count == current.Count &&
        previous.All(entry => current.TryGetValue(entry.Key, out var state) && state == entry.Value);
}
