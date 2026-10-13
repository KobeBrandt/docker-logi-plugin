namespace Loupedeck.DockerPlugin.Tests;

using Fakes;

using Helpers;

using Xunit;

public class DockerHttpClientFactoryTests
{
    private const String CliPath = "docker";

    private static String MissingSocketPath() => Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.sock");

    private static String MissingPipeName() => $"missing_{Guid.NewGuid():N}";

    [Fact]
    public void CreateNamedPipeClient_UsesLocalApiAddress() =>
        Assert.Equal(new Uri(DockerConstants.LocalApiBaseUrl), DockerHttpClientFactory.CreateNamedPipeClient(MissingPipeName()).BaseAddress);

    [Fact]
    public void CreateNamedPipeClient_NullPipeName_Throws() =>
        Assert.Throws<ArgumentNullException>(() => DockerHttpClientFactory.CreateNamedPipeClient(null));

    [Fact]
    public void CreateNamedPipeClient_MissingPipe_ApiReportedUnavailable() =>
        Assert.False(CreateApiClient(DockerHttpClientFactory.CreateNamedPipeClient(MissingPipeName())).IsDockerApiAvailable());

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

        Assert.False(CreateApiClient(httpClient).IsDockerApiAvailable());
    }

    private static DockerApiClient CreateApiClient(HttpClient httpClient) =>
        new(httpClient, FakeProcessRunner.ExitingWith(DockerConstants.CliSuccessExitCode), CliPath);
}
