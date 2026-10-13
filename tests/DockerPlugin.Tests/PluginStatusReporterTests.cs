namespace Loupedeck.DockerPlugin.Tests;

using Fakes;

using Helpers;

using Xunit;

public class PluginStatusReporterTests
{
    private readonly FakeDockerClient _client = new();
    private readonly PluginStatusReporter _reporter;
    private readonly List<(PluginStatus Status, String Message)> _reports = new();

    public PluginStatusReporterTests()
    {
        this._reporter = new PluginStatusReporter(this._client);
        this._reporter.StatusChanged += (status, message) => this._reports.Add((status, message));
    }

    [Fact]
    public void Constructor_NullClient_Throws() => Assert.Throws<ArgumentNullException>(() => new PluginStatusReporter(null));

    [Fact]
    public void CheckAndReport_DockerNotRunning_ReportsWarning()
    {
        this._client.DockerRunning = false;

        Assert.Equal(DockerReadiness.NotRunning, this._reporter.CheckAndReport());
        Assert.Equal((PluginStatus.Warning, PluginStatusReporter.DockerNotRunningMessage), Assert.Single(this._reports));
    }

    [Fact]
    public void CheckAndReport_ApiUnavailable_ReportsError()
    {
        this._client.ApiAvailable = false;

        Assert.Equal(DockerReadiness.ApiUnavailable, this._reporter.CheckAndReport());
        Assert.Equal((PluginStatus.Error, PluginStatusReporter.DockerApiUnavailableMessage), Assert.Single(this._reports));
    }

    [Fact]
    public void CheckAndReport_Ready_ReportsNormal()
    {
        Assert.Equal(DockerReadiness.Ready, this._reporter.CheckAndReport());
        Assert.Equal((PluginStatus.Normal, (String)null), Assert.Single(this._reports));
    }

    [Fact]
    public void CheckAndReport_SameReadinessTwice_ReportsOnce()
    {
        this._client.DockerRunning = false;

        this._reporter.CheckAndReport();
        this._reporter.CheckAndReport();

        Assert.Single(this._reports);
    }

    [Fact]
    public void CheckAndReport_DockerStartsAfterWarning_ClearsWarning()
    {
        this._client.DockerRunning = false;
        this._reporter.CheckAndReport();

        this._client.DockerRunning = true;
        this._reporter.CheckAndReport();

        Assert.Equal(PluginStatus.Normal, this._reports.Last().Status);
    }

    [Fact]
    public void CheckAndReport_NoSubscribers_DoesNotThrow()
    {
        var reporter = new PluginStatusReporter(new FakeDockerClient { DockerRunning = false });

        Assert.Equal(DockerReadiness.NotRunning, reporter.CheckAndReport());
    }

    [Theory]
    [InlineData(true, true, true)]
    [InlineData(false, true, false)]
    [InlineData(true, false, false)]
    public void EnsureDockerReady_ReturnsWhetherDockerIsReady(Boolean dockerRunning, Boolean apiAvailable, Boolean expected)
    {
        this._client.DockerRunning = dockerRunning;
        this._client.ApiAvailable = apiAvailable;

        Assert.Equal(expected, this._reporter.EnsureDockerReady());
    }
}
