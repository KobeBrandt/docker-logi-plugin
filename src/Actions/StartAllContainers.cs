namespace Loupedeck.DockerPlugin;

using Helpers;

public class StartAllContainers : PluginDynamicCommand
{
    private const String IconFileName = "play-solid-full.svg";

    public StartAllContainers()
        : base("Start all containers", "Start all the docker containers with a button press", String.Empty)
    {
    }

    protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
        BitmapHelper.MakeBitmapImage(IconFileName);

    protected override void RunCommand(String actionParameter)
    {
        if (this.Plugin.EnsureDockerReady())
        {
            DockerServices.Operations.StartAll();
        }
    }
}
