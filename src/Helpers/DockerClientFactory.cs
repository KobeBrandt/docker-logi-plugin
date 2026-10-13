namespace Loupedeck.DockerPlugin.Helpers;

public static class DockerClientFactory
{
    public static IDockerClient CreateForCurrentPlatform()
    {
        var locator = DockerPathLocator.CreateDefault();
        return new DockerApiClient(CreateHttpClient(locator), new ProcessRunner(), locator.FindCliExecutable());
    }

    // Docker Desktop on Windows serves the API on a named pipe; macOS and Linux serve it on a Unix socket.
    private static HttpClient CreateHttpClient(DockerPathLocator locator) =>
        OperatingSystem.IsWindows()
            ? DockerHttpClientFactory.CreateNamedPipeClient(DockerConstants.EnginePipeName)
            : DockerHttpClientFactory.CreateUnixSocketClient(locator.FindSocketPath);
}
