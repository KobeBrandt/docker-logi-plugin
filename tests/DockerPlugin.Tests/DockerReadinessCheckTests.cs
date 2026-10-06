namespace Loupedeck.DockerPlugin.Tests;

using Fakes;

using Helpers;

using Xunit;

public class DockerReadinessCheckTests
{
    [Theory]
    [InlineData(true, true, DockerReadiness.Ready)]
    [InlineData(false, true, DockerReadiness.NotRunning)]
    [InlineData(true, false, DockerReadiness.ApiUnavailable)]
    public void Check_MapsClientStateToReadiness(Boolean dockerRunning, Boolean apiAvailable, DockerReadiness expected)
    {
        var client = new FakeDockerClient { DockerRunning = dockerRunning, ApiAvailable = apiAvailable };

        Assert.Equal(expected, DockerReadinessCheck.Check(client));
    }
}
