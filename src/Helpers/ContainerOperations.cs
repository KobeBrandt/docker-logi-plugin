namespace Loupedeck.DockerPlugin.Helpers;

using Types;

public sealed class ContainerOperations
{
    private readonly IDockerClient _client;

    public ContainerOperations(IDockerClient client) =>
        this._client = client ?? throw new ArgumentNullException(nameof(client));

    public Boolean ToggleById(String containerId) =>
        this.ToggleFirstMatch(container => container.Id == containerId, containerId);

    public Boolean ToggleByDisplayName(String displayName) =>
        this.ToggleFirstMatch(container => ContainerQueries.GetDisplayName(container) == displayName, displayName);

    public Boolean StartAll() => this.ApplyToAllContainers(this._client.StartContainer);

    public Boolean StopAll() => this.ApplyToAllContainers(this._client.StopContainer);

    public Boolean ToggleComposeProject(String projectName)
    {
        var containers = this._client.GetAllContainers().Result;
        var projectContainers = containers == null ? [] : ContainerQueries.FilterByComposeProject(containers, projectName);
        if (projectContainers.Count == 0)
        {
            PluginLog.Warning($"No containers found for compose project {projectName}");
            return false;
        }

        var running = projectContainers.Where(ContainerQueries.IsRunning).ToList();
        var stopped = projectContainers.Except(running).ToList();
        // Majority vote: a mostly running stack is stopped, otherwise the stopped part is started.
        return running.Count > stopped.Count
            ? ApplyToEach(running, this._client.StopContainer)
            : ApplyToEach(stopped, this._client.StartContainer);
    }

    private Boolean Toggle(DockerContainer container)
    {
        var request = ContainerQueries.IsRunning(container)
            ? this._client.StopContainer(container.Id)
            : this._client.StartContainer(container.Id);
        return request.Result;
    }

    private Boolean ToggleFirstMatch(Func<DockerContainer, Boolean> predicate, String searchKey)
    {
        var container = this._client.GetAllContainers().Result?.FirstOrDefault(predicate);
        if (container == null)
        {
            PluginLog.Warning($"Container {searchKey} not found");
            return false;
        }

        return this.Toggle(container);
    }

    private Boolean ApplyToAllContainers(Func<String, Task<Boolean>> action)
    {
        var containers = this._client.GetAllContainers().Result;
        return containers != null && ApplyToEach(containers, action);
    }

    private static Boolean ApplyToEach(IEnumerable<DockerContainer> containers, Func<String, Task<Boolean>> action)
    {
        var results = containers.Select(container => action(container.Id).Result).ToList();
        return results.All(succeeded => succeeded);
    }
}
