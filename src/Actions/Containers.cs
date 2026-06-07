namespace Loupedeck.DockerPlugin;

public class Containers : PluginDynamicFolder
{
    public Containers()
    {
        this.DisplayName = "Containers";
        this.GroupName = "";
        this.Description = "A dynamic folder that shows all containers";
    }

    public override PluginDynamicFolderNavigation GetNavigationArea(DeviceType _) =>
        PluginDynamicFolderNavigation.ButtonArea;

    public override IEnumerable<String> GetButtonPressActionNames(DeviceType _)
    {
        var containers = DockerWhisperer.GetAllContainers().Result;
        if (containers == null)
        {
            return new[] { NavigateUpActionName };
        }

        var actions = new List<String> { NavigateUpActionName };
        actions.AddRange(containers.Select(c =>
            this.CreateCommandName(c.Names?.FirstOrDefault()?.TrimStart('/') ?? c.Id)));
        return actions;
    }

    public override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
    {
        if (actionParameter == NavigateUpActionName)
        {
            return "Back";
        }

        return actionParameter;
    }

    public override void RunCommand(String actionParameter)
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
            if (actionParameter == NavigateUpActionName)
            {
                return;
            }

            var containers = DockerWhisperer.GetAllContainers().Result;
            var container =
                containers?.FirstOrDefault(c => (c.Names?.FirstOrDefault()?.TrimStart('/') ?? c.Id) == actionParameter);
            if (container != null)
            {
                if (container.State == "running")
                {
                    DockerWhisperer.StopContainer(container.Id).Wait();
                }
                else
                {
                    DockerWhisperer.StartContainer(container.Id).Wait();
                }
            }
        }
        
    }
}