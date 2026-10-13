namespace Loupedeck.DockerPlugin;

using Helpers;

using Types;

public class ContainerStack : DockerToggleCommand
{
    public ContainerStack()
        : base("Stack", "Toggle all containers in a Docker stack", ActionIcons.Stack)
    {
    }

    protected override IEnumerable<String> GetParameterNames(List<DockerContainer> containers) =>
        ContainerQueries.GetComposeProjects(containers);

    protected override ToggleAction GetToggleAction(String parameterName) =>
        DockerServices.ToggleActions.ForComposeProject(parameterName);

    protected override Boolean Toggle(String parameterName) =>
        DockerServices.Operations.ToggleComposeProject(parameterName);
}
