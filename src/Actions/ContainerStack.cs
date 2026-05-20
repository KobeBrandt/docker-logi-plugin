namespace Loupedeck.DockerPlugin;

using Helpers;

public class ContainerStack : ActionEditorCommand
{
    private readonly Dictionary<String, String> _stacks = new();
    public ContainerStack()
    {
        this.Name = "ContainerStack";
        this.DisplayName = "Stack";
        this.Description = "Toggle all containers in a Docker stack";

        this.ActionEditor.AddControlEx(
            new ActionEditorListbox("Stack", "Stack"));

        this.ActionEditor.ListboxItemsRequested += this.OnListboxItemsRequested;
        this.ActionEditor.ControlValueChanged += this.OnControlValueChanged;
    }

    private void OnListboxItemsRequested(Object sender, ActionEditorListboxItemsRequestedEventArgs e)
    {
        if (e.ControlName.EqualsNoCase("Stack"))
        {
            var stacks = DockerWhisperer.GetAllComposeProjects();
            if (stacks != null)
            {
                foreach (var s in stacks)
                {
                    this._stacks[s] = s;
                    e.AddItem(s, s, s);
                }
            }
        }
    }
    
    private void OnControlValueChanged(Object sender, ActionEditorControlValueChangedEventArgs e)
    {
        if (e.ControlName.EqualsNoCase("Stack"))
        {
            var selectedStack = this._stacks[e.ActionEditorState.GetControlValue("Stack")];
            e.ActionEditorState.SetDisplayName(selectedStack);
        }
    }
    
    protected override BitmapImage GetCommandImage(ActionEditorActionParameters actionParameters, Int32 imageWidth, Int32 imageHeight)
    {
            return BitmapHelper.MakeBitmapImage("stack.svg", imageWidth);
    }

    protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
    {
        if (actionParameters.TryGetString("Stack", out var projectName))
        {
            var containers = DockerWhisperer.GetContainersByProject(projectName);
            if (containers != null && containers.Count > 0)
            {
                var running = containers.Where(c => c.State == "running").ToList();
                var stopped = containers.Where(c => c.State != "running").ToList();

                if (running.Count > stopped.Count)
                {
                    foreach (var c in running)
                    {
                        DockerWhisperer.StopContainer(c.Id).Wait();
                    }
                }
                else
                {
                    foreach (var c in stopped)
                    {
                        DockerWhisperer.StartContainer(c.Id).Wait();
                    }
                }
            }
        }
        return false;
    }
}