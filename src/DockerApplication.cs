namespace Loupedeck.DockerPlugin
{
    using System;

    public class DockerApplication : ClientApplication
    {
        protected override String GetProcessName() => String.Empty;

        protected override String GetBundleName() => String.Empty;

        public override ClientApplicationStatus GetApplicationStatus() => ClientApplicationStatus.Unknown;
    }
}
