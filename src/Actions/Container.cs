namespace Loupedeck.DockerPlugin;

using Helpers;

using Types;

public class Container : ActionEditorCommand
{
    private const String ContainerControlName = "Container";
    private const String IconFileName = "container.svg";
    private const String RunningSuffix = " [Running]";

    private readonly Dictionary<String, String> _containerNamesById = new();

    public Container()
    {
        this.Name = "Container";
        this.DisplayName = "Container";
        this.Description = "Toggle a Docker container on/off";

        this.ActionEditor.AddControlEx(new ActionEditorListbox(ContainerControlName, ContainerControlName));
        this.ActionEditor.ListboxItemsRequested += this.OnListboxItemsRequested;
        this.ActionEditor.ControlValueChanged += this.OnControlValueChanged;
    }

    private void OnListboxItemsRequested(Object sender, ActionEditorListboxItemsRequestedEventArgs e)
    {
        if (!e.ControlName.EqualsNoCase(ContainerControlName))
        {
            return;
        }

        var containers = DockerServices.Client.GetAllContainers().Result ?? [];
        foreach (var container in containers)
        {
            this.AddListboxItem(e, container);
        }
    }

    private void AddListboxItem(ActionEditorListboxItemsRequestedEventArgs e, DockerContainer container)
    {
        var name = ContainerQueries.GetDisplayName(container);
        var stateSuffix = ContainerQueries.IsRunning(container) ? RunningSuffix : String.Empty;
        this._containerNamesById[container.Id] = name;
        e.AddItem(container.Id, name, name + stateSuffix);
    }

    private void OnControlValueChanged(Object sender, ActionEditorControlValueChangedEventArgs e)
    {
        var containerId = e.ActionEditorState.GetControlValue(ContainerControlName);
        if (e.ControlName.EqualsNoCase(ContainerControlName) && this._containerNamesById.TryGetValue(containerId, out var name))
        {
            e.ActionEditorState.SetDisplayName(name);
        }
    }

    protected override BitmapImage GetCommandImage(ActionEditorActionParameters actionParameters, Int32 imageWidth, Int32 imageHeight) =>
        BitmapHelper.MakeBitmapImage(IconFileName);

    protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
    {
        if (!this.Plugin.EnsureDockerReady() || !actionParameters.TryGetString(ContainerControlName, out var containerId))
        {
            return false;
        }

        var toggled = DockerServices.Operations.ToggleById(containerId);
        this.ActionImageChanged();
        return toggled;
    }
}
