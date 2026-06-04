namespace Loupedeck.DockerPlugin;

using Helpers;

using Types;

public class StartAllContainers : PluginDynamicCommand
{
    private List<DockerContainer> _containers;

    public StartAllContainers()
        : base("Start all containers", "Start all the docker containers with a button press", "")
    {
    }

    protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
        BitmapHelper.MakeBitmapImage("play-solid-full.svg", imageSize);

    protected override void RunCommand(String actionParameter)
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
            this._containers = DockerWhisperer.GetAllContainers().Result;
            foreach (var container in this._containers)
            {
                DockerWhisperer.StartContainer(container.Id).Wait();
            }
        }
    }
}