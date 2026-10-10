namespace Loupedeck.DockerPlugin.Tests.Fakes;

using Helpers;

using Types;

public sealed class FakeDockerClient : IDockerClient
{
    public List<DockerContainer> Containers { get; set; } = new();

    public Boolean DockerRunning { get; set; } = true;

    public Boolean ApiAvailable { get; set; } = true;

    public HashSet<String> FailingContainerIds { get; } = new();

    public List<String> StartedIds { get; } = new();

    public List<String> StoppedIds { get; } = new();

    public Exception ListingFailure { get; set; }

    public Task<List<DockerContainer>> GetAllContainers() =>
        this.ListingFailure == null ? Task.FromResult(this.Containers) : Task.FromException<List<DockerContainer>>(this.ListingFailure);

    public Task<Boolean> StartContainer(String containerId) => this.Record(this.StartedIds, containerId);

    public Task<Boolean> StopContainer(String containerId) => this.Record(this.StoppedIds, containerId);

    public Boolean IsDockerRunning() => this.DockerRunning;

    public Boolean IsDockerApiAvailable() => this.ApiAvailable;

    private Task<Boolean> Record(List<String> log, String containerId)
    {
        log.Add(containerId);
        return Task.FromResult(!this.FailingContainerIds.Contains(containerId));
    }
}
