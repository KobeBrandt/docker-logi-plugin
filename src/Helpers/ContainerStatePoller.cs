namespace Loupedeck.DockerPlugin.Helpers;

public sealed class ContainerStatePoller : IDisposable
{
    private readonly Action _refresh;
    private readonly TimeSpan _interval;

    private CancellationTokenSource _cancellation;
    private Task _loop;

    public ContainerStatePoller(Action refresh, TimeSpan interval)
    {
        this._refresh = refresh ?? throw new ArgumentNullException(nameof(refresh));
        this._interval = interval;
    }

    public Boolean IsRunning => this._cancellation != null;

    public void Start()
    {
        if (this.IsRunning)
        {
            return;
        }

        this._cancellation = new CancellationTokenSource();
        this._loop = Task.Run(() => this.RunAsync(this._cancellation.Token));
    }

    public void Stop()
    {
        if (!this.IsRunning)
        {
            return;
        }

        this._cancellation.Cancel();
        this.WaitForLoopToExit();
        this._cancellation.Dispose();
        this._cancellation = null;
    }

    public void Dispose() => this.Stop();

    private async Task RunAsync(CancellationToken token)
    {
        using var timer = new PeriodicTimer(this._interval);
        do
        {
            this.RefreshSafely();
        }
        while (await WaitForNextTickAsync(timer, token));
    }

    private static async Task<Boolean> WaitForNextTickAsync(PeriodicTimer timer, CancellationToken token)
    {
        try
        {
            return await timer.WaitForNextTickAsync(token);
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    // A failing refresh must not end the loop, otherwise icons silently stop tracking Docker.
    private void RefreshSafely()
    {
        try
        {
            this._refresh();
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Refreshing container states failed");
        }
    }

    private void WaitForLoopToExit()
    {
        if (!this._loop.Wait(DockerConstants.PollerStopTimeout))
        {
            PluginLog.Warning("Container state poller did not stop within the timeout");
        }
    }
}
