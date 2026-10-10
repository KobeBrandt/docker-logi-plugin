namespace Loupedeck.DockerPlugin;

using Helpers;

public class Containers : PluginDynamicFolder
{
    private const String BackButtonDisplayName = "Back";

    public Containers()
    {
        this.DisplayName = "Containers";
        this.GroupName = String.Empty;
        this.Description = "A dynamic folder that shows all containers";
    }

    public override Boolean Load()
    {
        DockerServices.StateMonitor.StatesChanged += this.OnContainerStatesChanged;
        return base.Load();
    }

    public override Boolean Unload()
    {
        DockerServices.StateMonitor.StatesChanged -= this.OnContainerStatesChanged;
        return base.Unload();
    }

    private void OnContainerStatesChanged(Object sender, EventArgs e)
    {
        this.ButtonActionNamesChanged();
        foreach (var container in DockerServices.StateMonitor.Containers ?? [])
        {
            this.CommandImageChanged(ContainerQueries.GetDisplayName(container));
        }
    }

    public override PluginDynamicFolderNavigation GetNavigationArea(DeviceType _) =>
        PluginDynamicFolderNavigation.ButtonArea;

    public override IEnumerable<String> GetButtonPressActionNames(DeviceType _)
    {
        var containers = DockerServices.Client.GetAllContainers().Result ?? [];
        var containerActions = containers.Select(container => this.CreateCommandName(ContainerQueries.GetDisplayName(container)));
        return containerActions.Prepend(NavigateUpActionName).ToList();
    }

    public override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) =>
        actionParameter == NavigateUpActionName ? BackButtonDisplayName : actionParameter;

    public override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
    {
        if (actionParameter == NavigateUpActionName)
        {
            return base.GetCommandImage(actionParameter, imageSize);
        }

        var toggleAction = DockerServices.ToggleActions.ForDisplayName(actionParameter);
        return BitmapHelper.MakeBitmapImage(ActionIcons.ForToggle(toggleAction, ActionIcons.Container));
    }

    public override void RunCommand(String actionParameter)
    {
        if (this.Plugin.EnsureDockerReady() && actionParameter != NavigateUpActionName)
        {
            DockerServices.Operations.ToggleByDisplayName(actionParameter);
            DockerServices.StateMonitor.Refresh();
        }
    }
}
