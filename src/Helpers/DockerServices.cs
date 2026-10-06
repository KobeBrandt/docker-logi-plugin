namespace Loupedeck.DockerPlugin.Helpers;

public static class DockerServices
{
    public static IDockerClient Client { get; } = CreateDefaultClient();

    public static ContainerOperations Operations { get; } = new(Client);

    private static IDockerClient CreateDefaultClient()
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(DockerConstants.ApiBaseUrl),
            Timeout = DockerConstants.RequestTimeout,
        };
        return new DockerApiClient(httpClient, new ProcessRunner());
    }
}
