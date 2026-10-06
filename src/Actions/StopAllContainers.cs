namespace Loupedeck.DockerPlugin;

using Helpers;

public class StopAllContainers : PluginDynamicCommand
{
    private const String IconFileName = "stop-solid-full.svg";

    public StopAllContainers()
        : base("Stop all containers", "Stop all the docker containers with a button press", String.Empty)
    {
    }

    protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
        BitmapHelper.MakeBitmapImage(IconFileName);

    protected override void RunCommand(String actionParameter)
    {
        if (this.Plugin.EnsureDockerReady())
        {
            DockerServices.Operations.StopAll();
        }
    }
}
