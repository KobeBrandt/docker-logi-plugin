namespace Loupedeck.DockerPlugin;

// This class contains the plugin-level logic of the Loupedeck plugin.

public class DockerPlugin : Plugin
{
    // Initializes a new instance of the plugin class.
    public DockerPlugin()
    {
        // Initialize the plugin log.
        PluginLog.Init(this.Log);

        // Initialize the plugin resources.
        PluginResources.Init(this.Assembly);
    }

    // Gets a value indicating whether this is an API-only plugin.
    public override Boolean UsesApplicationApiOnly => true;

    // Gets a value indicating whether this is a Universal plugin or an Application plugin.
    public override Boolean HasNoApplication => true;

    // This method is called when the plugin is loaded.
    public override void Load()
    {
    }

    // This method is called when the plugin is unloaded.
    public override void Unload()
    {
    }
}