namespace Loupedeck.DockerPlugin.Helpers;

public static class DockerClientFactory
{
    public static IDockerClient CreateForCurrentPlatform()
    {
        var locator = DockerPathLocator.CreateDefault();
        return new DockerApiClient(CreateHttpClient(locator), new ProcessRunner(), locator.FindCliExecutable());
    }

    // Docker Desktop on Windows exposes the API over TCP; macOS and Linux only serve it on a Unix socket.
    private static HttpClient CreateHttpClient(DockerPathLocator locator) =>
        OperatingSystem.IsWindows()
            ? DockerHttpClientFactory.CreateTcpClient()
            : DockerHttpClientFactory.CreateUnixSocketClient(locator.FindSocketPath);
}
