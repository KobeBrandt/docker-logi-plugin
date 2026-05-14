namespace Loupedeck.DockerPlugin;

public static class ObjectExtensions
{
    public static void CheckNullArgument(this Object obj, String name)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(name);
        }
    }
}
