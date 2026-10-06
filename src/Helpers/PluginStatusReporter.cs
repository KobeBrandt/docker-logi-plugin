namespace Loupedeck.DockerPlugin.Helpers;

public static class PluginStatusReporter
{
    public const String DockerNotRunningMessage = "Docker not running";
    public const String DockerApiUnavailableMessage = "Docker API not found";

    public static Boolean EnsureDockerReady(this Plugin plugin)
    {
        var readiness = DockerReadinessCheck.Check(DockerServices.Client);
        switch (readiness)
        {
            case DockerReadiness.NotRunning:
                plugin.OnPluginStatusChanged(PluginStatus.Error, DockerNotRunningMessage);
                return false;
            case DockerReadiness.ApiUnavailable:
                plugin.OnPluginStatusChanged(PluginStatus.Error, DockerApiUnavailableMessage);
                return false;
            default:
                plugin.OnPluginStatusChanged(PluginStatus.Normal, null);
                return true;
        }
    }
}
