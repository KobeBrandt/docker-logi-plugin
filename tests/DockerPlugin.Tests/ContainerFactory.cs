namespace Loupedeck.DockerPlugin.Tests;

using Helpers;

using Types;

public static class ContainerFactory
{
    public const String StoppedState = "exited";

    public static DockerContainer Running(String id, String project = null) => Create(id, DockerConstants.RunningState, project);

    public static DockerContainer Stopped(String id, String project = null) => Create(id, StoppedState, project);

    private static DockerContainer Create(String id, String state, String project) => new()
    {
        Id = id,
        Names = [DockerConstants.ContainerNamePrefix + id],
        State = state,
        Labels = project == null ? null : new Dictionary<String, String> { [DockerConstants.ComposeProjectLabel] = project },
    };
}
