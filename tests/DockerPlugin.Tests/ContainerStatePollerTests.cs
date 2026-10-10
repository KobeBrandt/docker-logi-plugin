namespace Loupedeck.DockerPlugin.Tests;

using Helpers;

using Xunit;

public class ContainerStatePollerTests
{
    private const Int32 RefreshesToObserve = 3;

    private static readonly TimeSpan TestInterval = TimeSpan.FromMilliseconds(10);
    private static readonly TimeSpan SignalTimeout = TimeSpan.FromSeconds(5);

    private readonly CountdownEvent _refreshes = new(RefreshesToObserve);
    private Int32 _refreshCount;

    [Fact]
    public void Constructor_NullRefresh_Throws() =>
        Assert.Throws<ArgumentNullException>(() => new ContainerStatePoller(null, TestInterval));

    [Fact]
    public void Start_RefreshesRepeatedly()
    {
        using var poller = new ContainerStatePoller(this.CountRefresh, TestInterval);

        poller.Start();

        Assert.True(this._refreshes.Wait(SignalTimeout));
    }

    [Fact]
    public void Start_RefreshThrows_KeepsPolling()
    {
        using var poller = new ContainerStatePoller(this.CountRefreshThenThrow, TestInterval);

        poller.Start();

        Assert.True(this._refreshes.Wait(SignalTimeout));
    }

    [Fact]
    public void Stop_NoFurtherRefreshes()
    {
        var poller = new ContainerStatePoller(this.CountRefresh, TestInterval);
        poller.Start();
        Assert.True(this._refreshes.Wait(SignalTimeout));

        poller.Stop();
        var countAfterStop = Volatile.Read(ref this._refreshCount);
        Thread.Sleep(TestInterval * RefreshesToObserve);

        Assert.False(poller.IsRunning);
        Assert.Equal(countAfterStop, Volatile.Read(ref this._refreshCount));
    }

    [Fact]
    public void Stop_WhenNotStarted_DoesNothing()
    {
        var poller = new ContainerStatePoller(this.CountRefresh, TestInterval);

        poller.Stop();

        Assert.False(poller.IsRunning);
    }

    [Fact]
    public void Start_AfterStop_RestartsPolling()
    {
        using var poller = new ContainerStatePoller(this.CountRefresh, TestInterval);
        poller.Start();
        poller.Stop();
        this._refreshes.Reset();

        poller.Start();

        Assert.True(this._refreshes.Wait(SignalTimeout));
    }

    private void CountRefresh()
    {
        Interlocked.Increment(ref this._refreshCount);
        if (!this._refreshes.IsSet)
        {
            this._refreshes.Signal();
        }
    }

    private void CountRefreshThenThrow()
    {
        this.CountRefresh();
        throw new InvalidOperationException();
    }
}
