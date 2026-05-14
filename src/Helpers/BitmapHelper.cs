namespace Loupedeck.DockerPlugin.Helpers;

public static class BitmapHelper
{
    public static BitmapImage MakeBitmapImage(String path, PluginImageSize imageSize)
    {
        try
        {
            return PluginResources.ReadImage(path);
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "Failed to read image");
            return null;
        }
    }

    public static BitmapImage MakeBitmapImage(String path, Int32 imageWidth)
    {
        try
        {
            return PluginResources.ReadImage(path);
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "Failed to read image");
            return null;
        }
    }
}