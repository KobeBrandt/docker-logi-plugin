namespace Loupedeck.DockerPlugin.Helpers;

public static class BitmapHelper
{
    public static BitmapImage MakeBitmapImage(String resourceFileName)
    {
        try
        {
            return PluginResources.ReadImage(resourceFileName);
        }
        catch (FileNotFoundException ex)
        {
            PluginLog.Error(ex, $"Image resource {resourceFileName} not found");
            return null;
        }
    }
}
