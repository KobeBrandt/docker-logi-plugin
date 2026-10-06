namespace Loupedeck.DockerPlugin;

using Helpers;

public class ContainerStack : ActionEditorCommand
{
    private const String StackControlName = "Stack";
    private const String IconFileName = "stack.svg";

    public ContainerStack()
    {
        this.Name = "ContainerStack";
        this.DisplayName = "Stack";
        this.Description = "Toggle all containers in a Docker stack";

        this.ActionEditor.AddControlEx(new ActionEditorListbox(StackControlName, StackControlName));
        this.ActionEditor.ListboxItemsRequested += this.OnListboxItemsRequested;
        this.ActionEditor.ControlValueChanged += this.OnControlValueChanged;
    }

    private void OnListboxItemsRequested(Object sender, ActionEditorListboxItemsRequestedEventArgs e)
    {
        if (!e.ControlName.EqualsNoCase(StackControlName))
        {
            return;
        }

        var containers = DockerServices.Client.GetAllContainers().Result ?? [];
        foreach (var projectName in ContainerQueries.GetComposeProjects(containers))
        {
            e.AddItem(projectName, projectName, projectName);
        }
    }

    private void OnControlValueChanged(Object sender, ActionEditorControlValueChangedEventArgs e)
    {
        if (e.ControlName.EqualsNoCase(StackControlName))
        {
            e.ActionEditorState.SetDisplayName(e.ActionEditorState.GetControlValue(StackControlName));
        }
    }

    protected override BitmapImage GetCommandImage(ActionEditorActionParameters actionParameters, Int32 imageWidth, Int32 imageHeight) =>
        BitmapHelper.MakeBitmapImage(IconFileName);

    protected override Boolean RunCommand(ActionEditorActionParameters actionParameters) =>
        this.Plugin.EnsureDockerReady()
        && actionParameters.TryGetString(StackControlName, out var projectName)
        && DockerServices.Operations.ToggleComposeProject(projectName);
}
