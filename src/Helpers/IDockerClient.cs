namespace Loupedeck.DockerPlugin.Helpers;

using Types;

public interface IDockerClient
{
    Task<List<DockerContainer>> GetAllContainers();

    Task<Boolean> StartContainer(String containerId);

    Task<Boolean> StopContainer(String containerId);

    Boolean IsDockerRunning();

    Boolean IsDockerApiAvailable();
}
