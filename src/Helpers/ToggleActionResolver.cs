namespace Loupedeck.DockerPlugin.Helpers;

using Types;

public sealed class ToggleActionResolver
{
    private readonly ContainerStateMonitor _stateMonitor;

    public ToggleActionResolver(ContainerStateMonitor stateMonitor) =>
        this._stateMonitor = stateMonitor ?? throw new ArgumentNullException(nameof(stateMonitor));

    public ToggleAction ForDisplayName(String displayName) =>
        this.ForFirstMatch(ContainerQueries.HasDisplayName(displayName));

    public ToggleAction ForComposeProject(String projectName)
    {
        var containers = this._stateMonitor.Containers;
        var projectContainers = containers == null ? [] : ContainerQueries.FilterByComposeProject(containers, projectName);
        return projectContainers.Count == 0
            ? ToggleAction.Unknown
            : FromRunningState(ContainerQueries.IsMostlyRunning(projectContainers));
    }

    private ToggleAction ForFirstMatch(Func<DockerContainer, Boolean> predicate)
    {
        var container = this._stateMonitor.Containers?.FirstOrDefault(predicate);
        return container == null ? ToggleAction.Unknown : FromRunningState(ContainerQueries.IsRunning(container));
    }

    private static ToggleAction FromRunningState(Boolean isRunning) => isRunning ? ToggleAction.Stop : ToggleAction.Start;
}
