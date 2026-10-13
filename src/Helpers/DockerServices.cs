namespace Loupedeck.DockerPlugin.Helpers;

public static class DockerServices
{
    public static IDockerClient Client { get; } = DockerClientFactory.CreateForCurrentPlatform();

    public static ContainerOperations Operations { get; } = new(Client);

    public static ContainerStateMonitor StateMonitor { get; } = new(Client);

    public static ContainerStatePoller StatePoller { get; } = new(StateMonitor.Refresh, DockerConstants.ContainerStatePollInterval);

    public static ToggleActionResolver ToggleActions { get; } = new(StateMonitor);
}
