namespace Loupedeck.DockerPlugin;

using Helpers;

using Types;

public class StopAllContainers : PluginDynamicCommand
{
    private List<DockerContainer> _containers;

    public StopAllContainers()
        : base("Stop all containers", "Stop all the docker containers with a button press", "")
    {
    }

    protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
        BitmapHelper.MakeBitmapImage("stop-solid-full.svg", imageSize);

    protected override void RunCommand(String actionParameter)
    {
        this._containers = DockerWhisperer.GetAllContainers().Result;
        foreach (var container in this._containers)
        {
            DockerWhisperer.StopContainer(container.Id).Wait();
        }
    }
}