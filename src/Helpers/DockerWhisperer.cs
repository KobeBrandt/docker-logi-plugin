namespace Loupedeck.DockerPlugin;

using System.Net;
using System.Text.Json;

using Types;

public static class DockerWhisperer
{
    private const String Url = "http://localhost:2375/";
    private static readonly HttpClient Client = new() { Timeout = TimeSpan.FromSeconds(30) };

    public static async Task<List<DockerContainer>> GetAllContainers()
    {
        var extra = "containers/json?all=1";
        try
        {
            var response = await Client.GetAsync(Url + extra);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                // PluginLog.Info(responseBody);
                var json = JsonSerializer.Deserialize<List<DockerContainer>>(responseBody);
                return json;
            }

            PluginLog.Error($"Error: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            PluginLog.Error($"Exception: {ex.Message}");
            return null;
        }
    }

    public static async Task<Boolean> StartContainer(String containerId)
    {
        var extra = $"containers/{containerId}/start";
        PluginLog.Info($"Attempting to start container with ID: [{containerId}], URL: {Url + extra}");
        try
        {
            var response = await Client.PostAsync(Url + extra, null);
            if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotModified)
            {
                PluginLog.Info($"Container {containerId} started successfully");
                return true;
            }

            PluginLog.Error($"Error starting container: {response.StatusCode}");
            return false;
        }
        catch (Exception ex)
        {
            PluginLog.Error($"Exception starting container: {ex.Message}");
            return false;
        }
    }

    public static async Task<Boolean> StopContainer(String containerId)
    {
        var extra = $"containers/{containerId}/stop";
        PluginLog.Info($"Attempting to stop container with ID: [{containerId}], URL: {Url + extra}");
        try
        {
            var response = await Client.PostAsync(Url + extra, null);
            if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotModified)
            {
                PluginLog.Info($"Container {containerId} stopped successfully");
                return true;
            }

            PluginLog.Error($"Error stopping container: {response.StatusCode}");
            return false;
        }
        catch (Exception ex)
        {
            PluginLog.Error($"Exception stopping container: {ex.Message}");
            return false;
        }
    }

    public static List<String> GetAllComposeProjects()
    {
        var containers = GetAllContainers().Result;
        if (containers == null)
        {
            return null;
        }

        return containers
            .Where(c => c.Labels?.ContainsKey("com.docker.compose.project") == true)
            .Select(c => c.Labels["com.docker.compose.project"])
            .Distinct()
            .ToList();
    }

    public static List<DockerContainer> GetContainersByProject(String projectName)
    {
        var containers = GetAllContainers().Result;
        if (containers == null)
        {
            return null;
        }

        return containers.Where(c => c.Labels?.GetValueOrDefault("com.docker.compose.project") == projectName).ToList();
    }
}