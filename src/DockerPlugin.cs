namespace Loupedeck.DockerPlugin
{
    using System;

    using Helpers;

    public class DockerPlugin : Plugin
    {
        public override Boolean UsesApplicationApiOnly => true;

        public override Boolean HasNoApplication => true;

        public DockerPlugin()
        {
            PluginLog.Init(this.Log);
            PluginResources.Init(this.Assembly);
        }

        public override void Load() => DockerServices.StatePoller.Start();

        public override void Unload() => DockerServices.StatePoller.Stop();
    }
}