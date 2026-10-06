namespace Loupedeck.DockerPlugin.Tests;

using Helpers;

using Xunit;

public class BitmapHelperTests
{
    [Fact]
    public void MakeBitmapImage_MissingResource_ReturnsNull()
    {
        PluginResources.Init(typeof(BitmapHelperTests).Assembly);

        Assert.Null(BitmapHelper.MakeBitmapImage("missing.svg"));
    }
}
