namespace Loupedeck.DockerPlugin.Tests;

using System.ComponentModel;
using System.Net;

using Fakes;

using Helpers;

using Xunit;

public class DockerApiClientTests
{
    private const String ContainerId = "abc123";
    private const String ContainerListJson = """[{"Id":"abc123","Names":["/web"],"State":"running"}]""";
    private const Int32 FailedExitCode = 1;

    private static DockerApiClient CreateClient(FakeHttpMessageHandler handler, IProcessRunner runner = null) =>
        new(handler.CreateClient(), runner ?? FakeProcessRunner.ExitingWith(DockerConstants.CliSuccessExitCode));

    private static DockerApiClient CreateClient(IProcessRunner runner) =>
        CreateClient(FakeHttpMessageHandler.Returning(HttpStatusCode.OK), runner);

    [Fact]
    public void Constructor_NullHttpClient_Throws() =>
        Assert.Throws<ArgumentNullException>(() => new DockerApiClient(null, FakeProcessRunner.ExitingWith(0)));

    [Fact]
    public void Constructor_NullProcessRunner_Throws() =>
        Assert.Throws<ArgumentNullException>(() => new DockerApiClient(new HttpClient(), null));

    [Fact]
    public async Task GetAllContainers_Success_ParsesContainers()
    {
        var client = CreateClient(FakeHttpMessageHandler.Returning(HttpStatusCode.OK, ContainerListJson));

        var containers = await client.GetAllContainers();

        Assert.Equal(ContainerId, Assert.Single(containers).Id);
    }

    [Fact]
    public async Task GetAllContainers_ErrorStatus_ReturnsNull() =>
        Assert.Null(await CreateClient(FakeHttpMessageHandler.Returning(HttpStatusCode.InternalServerError)).GetAllContainers());

    [Fact]
    public async Task GetAllContainers_ConnectionFailure_ReturnsNull() =>
        Assert.Null(await CreateClient(FakeHttpMessageHandler.Throwing(new HttpRequestException())).GetAllContainers());

    [Fact]
    public async Task GetAllContainers_Timeout_ReturnsNull() =>
        Assert.Null(await CreateClient(FakeHttpMessageHandler.Throwing(new TaskCanceledException())).GetAllContainers());

    [Fact]
    public async Task GetAllContainers_InvalidJson_ReturnsNull() =>
        Assert.Null(await CreateClient(FakeHttpMessageHandler.Returning(HttpStatusCode.OK, "not json")).GetAllContainers());

    [Theory]
    [InlineData(HttpStatusCode.NoContent)]
    [InlineData(HttpStatusCode.NotModified)]
    public async Task StartContainer_SuccessOrAlreadyStarted_ReturnsTrue(HttpStatusCode statusCode)
    {
        var handler = FakeHttpMessageHandler.Returning(statusCode);

        Assert.True(await CreateClient(handler).StartContainer(ContainerId));
        Assert.EndsWith(String.Format(DockerConstants.StartContainerPathFormat, ContainerId), handler.Requests[0].RequestUri!.ToString());
    }

    [Fact]
    public async Task StopContainer_Success_PostsStopPath()
    {
        var handler = FakeHttpMessageHandler.Returning(HttpStatusCode.NoContent);

        Assert.True(await CreateClient(handler).StopContainer(ContainerId));
        Assert.EndsWith(String.Format(DockerConstants.StopContainerPathFormat, ContainerId), handler.Requests[0].RequestUri!.ToString());
    }

    [Fact]
    public async Task StopContainer_ErrorStatus_ReturnsFalse() =>
        Assert.False(await CreateClient(FakeHttpMessageHandler.Returning(HttpStatusCode.NotFound)).StopContainer(ContainerId));

    [Fact]
    public async Task StartContainer_ConnectionFailure_ReturnsFalse() =>
        Assert.False(await CreateClient(FakeHttpMessageHandler.Throwing(new HttpRequestException())).StartContainer(ContainerId));

    [Fact]
    public void IsDockerRunning_SuccessExitCode_ReturnsTrue() =>
        Assert.True(CreateClient(FakeProcessRunner.ExitingWith(DockerConstants.CliSuccessExitCode)).IsDockerRunning());

    [Fact]
    public void IsDockerRunning_FailedExitCode_ReturnsFalse() =>
        Assert.False(CreateClient(FakeProcessRunner.ExitingWith(FailedExitCode)).IsDockerRunning());

    [Fact]
    public void IsDockerRunning_CliMissing_ReturnsFalse() =>
        Assert.False(CreateClient(FakeProcessRunner.Throwing(new Win32Exception())).IsDockerRunning());

    [Fact]
    public void IsDockerRunning_ProcessNotStartable_ReturnsFalse() =>
        Assert.False(CreateClient(FakeProcessRunner.Throwing(new InvalidOperationException())).IsDockerRunning());

    [Fact]
    public void IsDockerApiAvailable_Success_ReturnsTrue() =>
        Assert.True(CreateClient(FakeHttpMessageHandler.Returning(HttpStatusCode.OK)).IsDockerApiAvailable());

    [Fact]
    public void IsDockerApiAvailable_ErrorStatus_ReturnsFalse() =>
        Assert.False(CreateClient(FakeHttpMessageHandler.Returning(HttpStatusCode.ServiceUnavailable)).IsDockerApiAvailable());

    [Fact]
    public void IsDockerApiAvailable_ConnectionFailure_ReturnsFalse() =>
        Assert.False(CreateClient(FakeHttpMessageHandler.Throwing(new HttpRequestException())).IsDockerApiAvailable());

    [Fact]
    public void IsDockerApiAvailable_Timeout_ReturnsFalse() =>
        Assert.False(CreateClient(FakeHttpMessageHandler.Throwing(new TaskCanceledException())).IsDockerApiAvailable());
}
