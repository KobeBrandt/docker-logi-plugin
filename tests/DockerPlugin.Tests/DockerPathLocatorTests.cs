namespace Loupedeck.DockerPlugin.Tests;

using Helpers;

using Xunit;

public class DockerPathLocatorTests
{
    private const String HomeDirectory = "/Users/tester";

    private static DockerPathLocator CreateLocator(params String[] existingPaths) =>
        new(existingPaths.Contains, HomeDirectory);

    private static String UserSocketPath(Int32 candidateIndex) =>
        Path.Combine(HomeDirectory, DockerConstants.UserSocketRelativePaths[candidateIndex]);

    [Fact]
    public void Constructor_NullPathExists_Throws() =>
        Assert.Throws<ArgumentNullException>(() => new DockerPathLocator(null, HomeDirectory));

    [Fact]
    public void Constructor_NullHomeDirectory_Throws() =>
        Assert.Throws<ArgumentNullException>(() => new DockerPathLocator(_ => true, null));

    [Fact]
    public void FindSocketPath_SystemSocketExists_PrefersSystemSocket() =>
        Assert.Equal(DockerConstants.SystemSocketPath, CreateLocator(DockerConstants.SystemSocketPath, UserSocketPath(0)).FindSocketPath());

    [Fact]
    public void FindSocketPath_OnlyUserSocketExists_ReturnsUserSocket() =>
        Assert.Equal(UserSocketPath(1), CreateLocator(UserSocketPath(1)).FindSocketPath());

    [Fact]
    public void FindSocketPath_NoSocketExists_FallsBackToSystemSocket() =>
        Assert.Equal(DockerConstants.SystemSocketPath, CreateLocator().FindSocketPath());

    [Fact]
    public void FindCliExecutable_InstalledCliExists_ReturnsFullPath()
    {
        var installedCli = DockerConstants.UnixCliCandidatePaths[1];

        Assert.Equal(installedCli, CreateLocator(installedCli).FindCliExecutable());
    }

    [Fact]
    public void FindCliExecutable_NoInstalledCli_FallsBackToPathLookup() =>
        Assert.Equal(DockerConstants.CliExecutable, CreateLocator().FindCliExecutable());
}
