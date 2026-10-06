namespace Loupedeck.DockerPlugin.Helpers;

using System.ComponentModel;
using System.Net;
using System.Text.Json;

using Types;

public sealed class DockerApiClient : IDockerClient
{
    private readonly HttpClient _httpClient;
    private readonly IProcessRunner _processRunner;
    private readonly String _cliExecutable;

    public DockerApiClient(HttpClient httpClient, IProcessRunner processRunner, String cliExecutable)
    {
        this._httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this._processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
        this._cliExecutable = cliExecutable ?? throw new ArgumentNullException(nameof(cliExecutable));
    }

    public async Task<List<DockerContainer>> GetAllContainers()
    {
        try
        {
            using var response = await this._httpClient.GetAsync(DockerConstants.ListAllContainersPath);
            if (!response.IsSuccessStatusCode)
            {
                PluginLog.Error($"Listing containers failed with status {response.StatusCode}");
                return null;
            }

            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<DockerContainer>>(body);
        }
        catch (Exception ex) when (IsRequestFailure(ex) || ex is JsonException)
        {
            PluginLog.Error(ex, "Listing containers failed");
            return null;
        }
    }

    public Task<Boolean> StartContainer(String containerId) =>
        this.PostContainerAction(DockerConstants.StartContainerPathFormat, containerId);

    public Task<Boolean> StopContainer(String containerId) =>
        this.PostContainerAction(DockerConstants.StopContainerPathFormat, containerId);

    public Boolean IsDockerRunning()
    {
        try
        {
            var exitCode = this._processRunner.RunAndGetExitCode(this._cliExecutable, DockerConstants.CliInfoArguments);
            return exitCode == DockerConstants.CliSuccessExitCode;
        }
        catch (Exception ex) when (ex is Win32Exception or InvalidOperationException)
        {
            PluginLog.Error(ex, "Docker CLI could not be started");
            return false;
        }
    }

    public Boolean IsDockerApiAvailable()
    {
        try
        {
            using var timeout = new CancellationTokenSource(DockerConstants.ApiProbeTimeout);
            using var response = this._httpClient.GetAsync(DockerConstants.VersionPath, timeout.Token).GetAwaiter().GetResult();
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) when (IsRequestFailure(ex))
        {
            PluginLog.Warning($"Docker API not reachable: {ex.Message}");
            return false;
        }
    }

    private async Task<Boolean> PostContainerAction(String pathFormat, String containerId)
    {
        var path = String.Format(pathFormat, containerId);
        try
        {
            using var response = await this._httpClient.PostAsync(path, null);
            // Docker answers 304 when the container is already in the requested state.
            if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotModified)
            {
                return true;
            }

            PluginLog.Error($"Request {path} failed with status {response.StatusCode}");
            return false;
        }
        catch (Exception ex) when (IsRequestFailure(ex))
        {
            PluginLog.Error(ex, $"Request {path} failed");
            return false;
        }
    }

    private static Boolean IsRequestFailure(Exception ex) => ex is HttpRequestException or TaskCanceledException;
}
