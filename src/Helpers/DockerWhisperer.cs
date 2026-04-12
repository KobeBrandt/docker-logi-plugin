namespace Loupedeck.DockerPlugin;

using System.Text.Json;

using Helpers;

using Types;

public static class DockerWhisperer
{
    private static readonly HttpClient Client = new HttpClient();
    private const String Url = "http://localhost:2375/";

    public static async Task<List<DockerContainer>> GetAllContainers()
    {
        String extra = "containers/json?all=1";
        try
        {
            HttpResponseMessage response = await Client.GetAsync(Url + extra);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                PluginLog.Info($"Response: {responseBody}");
                var json = JsonSerializer.Deserialize<DockerContainers>(responseBody);
                return json.Containers;
            }
            else
            {
                PluginLog.Error($"Error: {response.StatusCode}");
                return null;
            }
        }
        catch (Exception ex)
        {
            PluginLog.Error($"Exception: {ex.Message}");
            return null;
        }
    }
}