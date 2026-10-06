namespace Loupedeck.DockerPlugin.Helpers;

public static class DockerServices
{
    public static IDockerClient Client { get; } = DockerClientFactory.CreateForCurrentPlatform();

    public static ContainerOperations Operations { get; } = new(Client);
}
