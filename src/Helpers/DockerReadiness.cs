namespace Loupedeck.DockerPlugin.Helpers;

public enum DockerReadiness
{
    Ready,
    NotRunning,
    ApiUnavailable,
}

public static class DockerReadinessCheck
{
    public static DockerReadiness Check(IDockerClient client)
    {
        if (!client.IsDockerRunning())
        {
            return DockerReadiness.NotRunning;
        }

        return client.IsDockerApiAvailable() ? DockerReadiness.Ready : DockerReadiness.ApiUnavailable;
    }
}
