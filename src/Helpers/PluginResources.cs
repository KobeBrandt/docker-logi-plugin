namespace Loupedeck.DockerPlugin
{
    using System;
    using System.Reflection;

    internal static class PluginResources
    {
        private static Assembly _assembly;

        public static void Init(Assembly assembly)
        {
            assembly.CheckNullArgument(nameof(assembly));
            PluginResources._assembly = assembly;
        }

        public static String FindFile(String fileName) => PluginResources._assembly.FindFileOrThrow(fileName);

        public static BitmapImage ReadImage(String resourceName) => PluginResources._assembly.ReadImage(PluginResources.FindFile(resourceName));
    }
}
