namespace Loupedeck.DockerPlugin;

using Helpers;

public class StopAllContainers : PluginDynamicCommand
{
    public StopAllContainers()
        : base("Stop all containers", "Stop all the docker containers with a button press", String.Empty)
    {
    }

    protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
        BitmapHelper.MakeBitmapImage(ActionIcons.Stop);

    protected override void RunCommand(String actionParameter)
    {
        if (this.Plugin.EnsureDockerReady())
        {
            DockerServices.Operations.StopAll();
        }
    }
}
