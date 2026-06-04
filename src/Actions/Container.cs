namespace Loupedeck.DockerPlugin;

using Helpers;

public class Container : ActionEditorCommand
{
    private readonly Dictionary<String, String> _containerStates = new();
    private readonly Dictionary<String, String> _containers = new();

    public Container()
    {
        this.Name = "Container";
        this.DisplayName = "Container";
        this.Description = "Toggle a Docker container on/off";

        this.ActionEditor.AddControlEx(
            new ActionEditorListbox("Container", "Container"));

        this.ActionEditor.ListboxItemsRequested += this.OnListboxItemsRequested;
        this.ActionEditor.ControlValueChanged += this.OnControlValueChanged;
    }

    private void OnListboxItemsRequested(Object sender, ActionEditorListboxItemsRequestedEventArgs e)
    {
        if (e.ControlName.EqualsNoCase("Container"))
        {
            var containers = DockerWhisperer.GetAllContainers().Result;
            if (containers != null)
            {
                foreach (var c in containers)
                {
                    var name = c.Names?.FirstOrDefault()?.TrimStart('/') ?? c.Id;
                    var running = c.State == "running" ? " [Running]" : "";
                    this._containers[c.Id] = name;
                    this._containerStates[c.Id] = c.State ?? "unknown";
                    e.AddItem(c.Id, name, $"{name}{running}");
                }
            }
        }
    }

    private void OnControlValueChanged(Object sender, ActionEditorControlValueChangedEventArgs e)
    {
        if (e.ControlName.EqualsNoCase("Container"))
        {
            var selectedContainer = this._containers[e.ActionEditorState.GetControlValue("Container")];
            e.ActionEditorState.SetDisplayName(selectedContainer);
        }
    }

    protected override BitmapImage GetCommandImage(ActionEditorActionParameters actionParameters, Int32 imageWidth,
        Int32 imageHeight)
    {
        return BitmapHelper.MakeBitmapImage("container.svg", imageWidth);
    }

    protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
    {
        if (!DockerWhisperer.IsDockerRunning())
        {
            this.Plugin.OnPluginStatusChanged(Loupedeck.PluginStatus.Error, "Docker not running");
        }
        else if (!DockerWhisperer.IsDockerApiAvailable())
        {
            this.Plugin.OnPluginStatusChanged(Loupedeck.PluginStatus.Error, "Docker API not found");
        }
        else
        {
            this.Plugin.OnPluginStatusChanged(Loupedeck.PluginStatus.Normal, null);
            if (actionParameters.TryGetString("Container", out var containerId))
            {
                var containers = DockerWhisperer.GetAllContainers().Result;
                var container = containers?.FirstOrDefault(c => c.Id == containerId);
                if (container != null)
                {
                    PluginLog.Info($"Container {container.Id} state: {container.State}");
                    if (container.State == "running")
                    {
                        var result = DockerWhisperer.StopContainer(container.Id).Result;
                        _containerStates[container.Id] = "stopped";
                        PluginLog.Info($"Stop result: {result}");
                    }
                    else
                    {
                        var result = DockerWhisperer.StartContainer(container.Id).Result;
                        _containerStates[container.Id] = "running";
                        PluginLog.Info($"Start result: {result}");
                    }

                    this.ActionImageChanged();
                    return true;
                }
            }
        }
        return false;
    }
}