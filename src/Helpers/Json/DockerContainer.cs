namespace Loupedeck.DockerPlugin.Types;

using System.Text.Json.Serialization;

public class DockerContainer
{
    [JsonPropertyName("Id")] public String Id { get; set; }

    [JsonPropertyName("Names")] public List<String> Names { get; set; }

    [JsonPropertyName("State")] public String State { get; set; }

    [JsonPropertyName("Labels")] public Dictionary<String, String> Labels { get; set; }
}