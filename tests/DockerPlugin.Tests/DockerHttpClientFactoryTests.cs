namespace Loupedeck.DockerPlugin.Tests;

using Fakes;

using Helpers;

using Xunit;

public class DockerHttpClientFactoryTests
{
    private const String CliPath = "docker";

    private static String MissingSocketPath() => Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.sock");

    [Fact]
    public void CreateTcpClient_UsesTcpApiAddress() =>
        Assert.Equal(new Uri(DockerConstants.ApiBaseUrl), DockerHttpClientFactory.CreateTcpClient().BaseAddress);

    [Fact]
    public void CreateUnixSocketClient_UsesRequestTimeout() =>
        Assert.Equal(DockerConstants.RequestTimeout, DockerHttpClientFactory.CreateUnixSocketClient(MissingSocketPath).Timeout);

    [Fact]
    public void CreateUnixSocketClient_NullProvider_Throws() =>
        Assert.Throws<ArgumentNullException>(() => DockerHttpClientFactory.CreateUnixSocketClient(null));

    [Fact]
    public void CreateUnixSocketClient_MissingSocket_ApiReportedUnavailable()
    {
        var httpClient = DockerHttpClientFactory.CreateUnixSocketClient(MissingSocketPath);
        var client = new DockerApiClient(httpClient, FakeProcessRunner.ExitingWith(DockerConstants.CliSuccessExitCode), CliPath);

        Assert.False(client.IsDockerApiAvailable());
    }
}
