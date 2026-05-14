namespace Loupedeck.DockerPlugin;

public class ContainerStack : ActionEditorCommand
{
    public ContainerStack()
    {
        this.Name = "ContainerStack";
        this.DisplayName = "Container Stack";
        this.GroupName = "Docker";
        this.Description = "Toggle all containers in a Docker Compose stack";

        this.ActionEditor.AddControlEx(
            new ActionEditorListbox("Service", "Service"));

        this.ActionEditor.ListboxItemsRequested += this.OnListboxItemsRequested;
    }

    private void OnListboxItemsRequested(Object sender, ActionEditorListboxItemsRequestedEventArgs e)
    {
        if (e.ControlName.EqualsNoCase("Service"))
        {
            var projects = DockerWhisperer.GetAllComposeProjects();
            if (projects != null)
            {
                foreach (var project in projects)
                {
                    e.AddItem(project, project, project);
                }
            }
        }
    }

    protected override Boolean RunCommand(ActionEditorActionParameters actionParameters)
    {
        if (actionParameters.TryGetString("Service", out var projectName))
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

                return true;
            }
        }

        return false;
    }
}