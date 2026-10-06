namespace Loupedeck.DockerPlugin
{
    using System;

    public class DockerPlugin : Plugin
    {
        public override Boolean UsesApplicationApiOnly => true;

        public override Boolean HasNoApplication => true;

        public DockerPlugin()
        {
            PluginLog.Init(this.Log);
            PluginResources.Init(this.Assembly);
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }
    }
}