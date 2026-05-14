namespace Loupedeck.DockerPlugin;

using Helpers;

using Types;

public class StartAllContainers : PluginDynamicCommand
{
    private List<DockerContainer> _containers;

    public StartAllContainers()
        : base("Start all containers", "Start all the docker containers with a button press", "Containers")
    {
    }

    protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
        BitmapHelper.MakeBitmapImage("play-solid-full.svg", imageSize);

    protected override void RunCommand(String actionParameter)
    {
        this._containers = DockerWhisperer.GetAllContainers().Result;
        foreach (var container in this._containers)
        {
            DockerWhisperer.StartContainer(container.Id).Wait();
        }
    }
}