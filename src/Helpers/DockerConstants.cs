namespace Loupedeck.DockerPlugin.Helpers;

public static class DockerConstants
{
    public const String ApiBaseUrl = "http://localhost:2375/";
    public const String UnixSocketApiBaseUrl = "http://localhost/";
    public const String SystemSocketPath = "/var/run/docker.sock";
    public const String ListAllContainersPath = "containers/json?all=1";
    public const String VersionPath = "version";
    public const String StartContainerPathFormat = "containers/{0}/start";
    public const String StopContainerPathFormat = "containers/{0}/stop";

    public const String RunningState = "running";
    public const String ComposeProjectLabel = "com.docker.compose.project";
    public const Char ContainerNamePrefix = '/';

    public const String CliExecutable = "docker";
    public const String CliInfoArguments = "info";
    public const Int32 CliSuccessExitCode = 0;

    public static readonly String[] UserSocketRelativePaths =
    [
        ".docker/run/docker.sock",
        ".orbstack/run/docker.sock",
        ".colima/default/docker.sock",
    ];

    public static readonly String[] UnixCliCandidatePaths =
    [
        "/usr/local/bin/docker",
        "/opt/homebrew/bin/docker",
        "/Applications/Docker.app/Contents/Resources/bin/docker",
        "/usr/bin/docker",
    ];

    public static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(30);
    public static readonly TimeSpan ApiProbeTimeout = TimeSpan.FromSeconds(1);
}
