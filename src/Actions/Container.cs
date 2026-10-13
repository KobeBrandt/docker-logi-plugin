namespace Loupedeck.DockerPlugin;

using Helpers;

using Types;

public class Container : DockerToggleCommand
{
    public Container()
        : base("Container", "Toggle a Docker container on/off", ActionIcons.Container)
    {
    }

    protected override IEnumerable<String> GetParameterNames(List<DockerContainer> containers) =>
        containers.Select(ContainerQueries.GetDisplayName);

    protected override ToggleAction GetToggleAction(String parameterName) =>
        DockerServices.ToggleActions.ForDisplayName(parameterName);

    protected override Boolean Toggle(String parameterName) =>
        DockerServices.Operations.ToggleByDisplayName(parameterName);
}
