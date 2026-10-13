namespace Loupedeck.DockerPlugin.Helpers;

using Types;

public static class ContainerQueries
{
    public static String GetDisplayName(DockerContainer container) =>
        container.Names?.FirstOrDefault()?.TrimStart(DockerConstants.ContainerNamePrefix) ?? container.Id;

    public static Boolean IsRunning(DockerContainer container) => container.State == DockerConstants.RunningState;

    public static Func<DockerContainer, Boolean> HasDisplayName(String displayName) =>
        container => GetDisplayName(container) == displayName;

    // Majority vote: a stack that is mostly running counts as running, so toggling it stops it.
    public static Boolean IsMostlyRunning(IReadOnlyCollection<DockerContainer> containers) =>
        containers.Count(IsRunning) > containers.Count(container => !IsRunning(container));

    public static List<String> GetComposeProjects(IEnumerable<DockerContainer> containers) =>
        containers
            .Select(GetComposeProject)
            .Where(project => project != null)
            .Distinct()
            .ToList();

    public static List<DockerContainer> FilterByComposeProject(IEnumerable<DockerContainer> containers, String projectName) =>
        containers.Where(container => GetComposeProject(container) == projectName).ToList();

    private static String GetComposeProject(DockerContainer container) =>
        container.Labels?.GetValueOrDefault(DockerConstants.ComposeProjectLabel);
}
