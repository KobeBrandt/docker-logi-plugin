namespace Loupedeck.DockerPlugin.Tests;

using Fakes;

using Helpers;

using Xunit;

public class ContainerOperationsTests
{
    private const String Project = "stack";

    private readonly FakeDockerClient _client = new();

    private ContainerOperations Operations => new(this._client);

    [Fact]
    public void Constructor_NullClient_Throws() => Assert.Throws<ArgumentNullException>(() => new ContainerOperations(null));

    [Fact]
    public void ToggleById_RunningContainer_StopsIt()
    {
        this._client.Containers = [ContainerFactory.Running("a")];

        Assert.True(this.Operations.ToggleById("a"));
        Assert.Equal(["a"], this._client.StoppedIds);
    }

    [Fact]
    public void ToggleByDisplayName_StoppedContainer_StartsIt()
    {
        this._client.Containers = [ContainerFactory.Stopped("a")];

        Assert.True(this.Operations.ToggleByDisplayName("a"));
        Assert.Equal(["a"], this._client.StartedIds);
    }

    [Fact]
    public void ToggleById_UnknownContainer_ReturnsFalse() => Assert.False(this.Operations.ToggleById("missing"));

    [Fact]
    public void ToggleById_ListingFailed_ReturnsFalse()
    {
        this._client.Containers = null;

        Assert.False(this.Operations.ToggleById("a"));
    }

    [Fact]
    public void ToggleById_DockerRejectsRequest_ReturnsFalse()
    {
        this._client.Containers = [ContainerFactory.Stopped("a")];
        this._client.FailingContainerIds.Add("a");

        Assert.False(this.Operations.ToggleById("a"));
    }

    [Fact]
    public void StartAll_StartsEveryContainer()
    {
        this._client.Containers = [ContainerFactory.Stopped("a"), ContainerFactory.Stopped("b")];

        Assert.True(this.Operations.StartAll());
        Assert.Equal(["a", "b"], this._client.StartedIds);
    }

    [Fact]
    public void StopAll_OneFailure_StillAttemptsAllAndReturnsFalse()
    {
        this._client.Containers = [ContainerFactory.Running("a"), ContainerFactory.Running("b")];
        this._client.FailingContainerIds.Add("a");

        Assert.False(this.Operations.StopAll());
        Assert.Equal(["a", "b"], this._client.StoppedIds);
    }

    [Fact]
    public void StartAll_ListingFailed_ReturnsFalse()
    {
        this._client.Containers = null;

        Assert.False(this.Operations.StartAll());
    }

    [Fact]
    public void ToggleComposeProject_MostlyRunning_StopsRunningContainers()
    {
        this._client.Containers = [ContainerFactory.Running("a", Project), ContainerFactory.Running("b", Project), ContainerFactory.Stopped("c", Project)];

        Assert.True(this.Operations.ToggleComposeProject(Project));
        Assert.Equal(["a", "b"], this._client.StoppedIds);
    }

    [Fact]
    public void ToggleComposeProject_MostlyStopped_StartsStoppedContainers()
    {
        this._client.Containers = [ContainerFactory.Running("a", Project), ContainerFactory.Stopped("b", Project)];

        Assert.True(this.Operations.ToggleComposeProject(Project));
        Assert.Equal(["b"], this._client.StartedIds);
    }

    [Fact]
    public void ToggleComposeProject_UnknownProject_ReturnsFalse()
    {
        this._client.Containers = [ContainerFactory.Running("a", Project)];

        Assert.False(this.Operations.ToggleComposeProject("other"));
    }

    [Fact]
    public void ToggleComposeProject_ListingFailed_ReturnsFalse()
    {
        this._client.Containers = null;

        Assert.False(this.Operations.ToggleComposeProject(Project));
    }
}
