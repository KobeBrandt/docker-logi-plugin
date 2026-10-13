namespace Loupedeck.DockerPlugin.Helpers;

public sealed class PluginStatusReporter
{
    public const String DockerNotRunningMessage = "Docker is not running. Start Docker to use container actions.";
    public const String DockerApiUnavailableMessage = "Docker API not found";

    private readonly IDockerClient _client;
    private readonly Object _sync = new();

    private DockerReadiness? _lastReportedReadiness;

    public PluginStatusReporter(IDockerClient client) =>
        this._client = client ?? throw new ArgumentNullException(nameof(client));

    public event Action<PluginStatus, String> StatusChanged;

    public Boolean EnsureDockerReady() => this.CheckAndReport() == DockerReadiness.Ready;

    public DockerReadiness CheckAndReport()
    {
        var readiness = DockerReadinessCheck.Check(this._client);
        if (this.StoreIfChanged(readiness))
        {
            this.Report(readiness);
        }

        return readiness;
    }

    // Reporting only on change keeps the background poll from re-sending the same status every tick.
    private Boolean StoreIfChanged(DockerReadiness readiness)
    {
        lock (this._sync)
        {
            if (this._lastReportedReadiness == readiness)
            {
                return false;
            }

            this._lastReportedReadiness = readiness;
            return true;
        }
    }

    private void Report(DockerReadiness readiness)
    {
        switch (readiness)
        {
            case DockerReadiness.NotRunning:
                this.StatusChanged?.Invoke(PluginStatus.Warning, DockerNotRunningMessage);
                break;
            case DockerReadiness.ApiUnavailable:
                this.StatusChanged?.Invoke(PluginStatus.Error, DockerApiUnavailableMessage);
                break;
            default:
                this.StatusChanged?.Invoke(PluginStatus.Normal, null);
                break;
        }
    }
}
