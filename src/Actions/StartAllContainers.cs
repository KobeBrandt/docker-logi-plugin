namespace Loupedeck.DockerPlugin;

using Helpers;

public class StartAllContainers : PluginDynamicCommand
{
    public StartAllContainers()
        : base("Start all containers", "Start all the docker containers with a button press", String.Empty)
    {
    }

    protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
        BitmapHelper.MakeBitmapImage(ActionIcons.Start);

    protected override void RunCommand(String actionParameter)
    {
        if (this.Plugin.EnsureDockerReady())
        {
            DockerServices.Operations.StartAll();
        }
    }
}
