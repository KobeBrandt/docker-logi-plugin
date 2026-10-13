namespace Loupedeck.DockerPlugin.Tests;

using Fakes;

using Helpers;

using Xunit;

public class ContainerStateMonitorTests
{
    private readonly FakeDockerClient _client = new();
    private readonly ContainerStateMonitor _monitor;
    private Int32 _changeCount;

    public ContainerStateMonitorTests()
    {
        this._monitor = new ContainerStateMonitor(this._client);
        this._monitor.StatesChanged += (_, _) => this._changeCount++;
    }

    [Fact]
    public void Constructor_NullClient_Throws() => Assert.Throws<ArgumentNullException>(() => new ContainerStateMonitor(null));

    [Fact]
    public void Containers_BeforeRefresh_IsNull() => Assert.Null(this._monitor.Containers);

    [Fact]
    public void Refresh_FirstListingWithContainers_RaisesChangeAndCachesList()
    {
        this._client.Containers = [ContainerFactory.Running("a")];

        this._monitor.Refresh();

        Assert.Equal(1, this._changeCount);
        Assert.Same(this._client.Containers, this._monitor.Containers);
    }

    [Fact]
    public void Refresh_SameStates_DoesNotRaiseChange()
    {
        this._client.Containers = [ContainerFactory.Running("a")];
        this._monitor.Refresh();

        this._client.Containers = [ContainerFactory.Running("a")];
        this._monitor.Refresh();

        Assert.Equal(1, this._changeCount);
    }

    [Fact]
    public void Refresh_ContainerStoppedExternally_RaisesChange()
    {
        this._client.Containers = [ContainerFactory.Running("a")];
        this._monitor.Refresh();

        this._client.Containers = [ContainerFactory.Stopped("a")];
        this._monitor.Refresh();

        Assert.Equal(2, this._changeCount);
    }

    [Fact]
    public void Refresh_ContainerAdded_RaisesChange()
    {
        this._client.Containers = [ContainerFactory.Running("a")];
        this._monitor.Refresh();

        this._client.Containers = [ContainerFactory.Running("a"), ContainerFactory.Stopped("b")];
        this._monitor.Refresh();

        Assert.Equal(2, this._changeCount);
    }

    [Fact]
    public void Refresh_ContainerReplacedBySameCount_RaisesChange()
    {
        this._client.Containers = [ContainerFactory.Running("a")];
        this._monitor.Refresh();

        this._client.Containers = [ContainerFactory.Running("b")];
        this._monitor.Refresh();

        Assert.Equal(2, this._changeCount);
    }

    [Fact]
    public void Refresh_ListingFailsAfterSuccess_RaisesChangeAndClearsCache()
    {
        this._client.Containers = [ContainerFactory.Running("a")];
        this._monitor.Refresh();

        this._client.Containers = null;
        this._monitor.Refresh();

        Assert.Equal(2, this._changeCount);
        Assert.Null(this._monitor.Containers);
    }

    [Fact]
    public void Refresh_ListingKeepsFailing_DoesNotRaiseChange()
    {
        this._client.Containers = null;

        this._monitor.Refresh();
        this._monitor.Refresh();

        Assert.Equal(0, this._changeCount);
    }

    [Fact]
    public void Refresh_ListingThrows_PropagatesWithoutRaisingChange()
    {
        this._client.ListingFailure = new InvalidOperationException();

        Assert.Throws<AggregateException>(this._monitor.Refresh);
        Assert.Equal(0, this._changeCount);
    }

    [Fact]
    public void RefreshIfNeverLoaded_NotLoaded_LoadsContainers()
    {
        this._client.Containers = [ContainerFactory.Running("a")];

        this._monitor.RefreshIfNeverLoaded();

        Assert.Same(this._client.Containers, this._monitor.Containers);
    }

    [Fact]
    public void RefreshIfNeverLoaded_AlreadyLoaded_KeepsCachedList()
    {
        this._client.Containers = [ContainerFactory.Running("a")];
        this._monitor.Refresh();
        var cached = this._monitor.Containers;
        this._client.Containers = [ContainerFactory.Stopped("a")];

        this._monitor.RefreshIfNeverLoaded();

        Assert.Same(cached, this._monitor.Containers);
    }
}
