namespace Loupedeck.DockerPlugin.Tests;

using Fakes;

using Helpers;

using Xunit;

public class ToggleActionResolverTests
{
    private const String Project = "stack";

    private readonly FakeDockerClient _client = new();

    private ToggleActionResolver Resolver
    {
        get
        {
            var stateMonitor = new ContainerStateMonitor(this._client);
            stateMonitor.Refresh();
            return new ToggleActionResolver(stateMonitor);
        }
    }

    [Fact]
    public void Constructor_NullClient_Throws() => Assert.Throws<ArgumentNullException>(() => new ToggleActionResolver(null));

    [Fact]
    public void ForDisplayName_RunningContainer_ReturnsStop()
    {
        this._client.Containers = [ContainerFactory.Running("a")];

        Assert.Equal(ToggleAction.Stop, this.Resolver.ForDisplayName("a"));
    }

    [Fact]
    public void ForDisplayName_StoppedContainer_ReturnsStart()
    {
        this._client.Containers = [ContainerFactory.Stopped("a")];

        Assert.Equal(ToggleAction.Start, this.Resolver.ForDisplayName("a"));
    }

    [Fact]
    public void ForDisplayName_UnknownContainer_ReturnsUnknown() =>
        Assert.Equal(ToggleAction.Unknown, this.Resolver.ForDisplayName("missing"));

    [Fact]
    public void ForDisplayName_ListingFailed_ReturnsUnknown()
    {
        this._client.Containers = null;

        Assert.Equal(ToggleAction.Unknown, this.Resolver.ForDisplayName("a"));
    }

    [Fact]
    public void ForComposeProject_MostlyRunning_ReturnsStop()
    {
        this._client.Containers = [ContainerFactory.Running("a", Project), ContainerFactory.Running("b", Project), ContainerFactory.Stopped("c", Project)];

        Assert.Equal(ToggleAction.Stop, this.Resolver.ForComposeProject(Project));
    }

    [Fact]
    public void ForComposeProject_EvenSplit_ReturnsStart()
    {
        this._client.Containers = [ContainerFactory.Running("a", Project), ContainerFactory.Stopped("b", Project)];

        Assert.Equal(ToggleAction.Start, this.Resolver.ForComposeProject(Project));
    }

    [Fact]
    public void ForComposeProject_UnknownProject_ReturnsUnknown() =>
        Assert.Equal(ToggleAction.Unknown, this.Resolver.ForComposeProject(Project));

    [Fact]
    public void ForComposeProject_ListingFailed_ReturnsUnknown()
    {
        this._client.Containers = null;

        Assert.Equal(ToggleAction.Unknown, this.Resolver.ForComposeProject(Project));
    }
}
