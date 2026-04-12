namespace Loupedeck.DockerPlugin.Types;

using System.Text.Json.Serialization;

public class DockerContainer
{
    [JsonPropertyName("id")]
    public String Id { get; set; }

    [JsonPropertyName("state")]
    public String State { get; set; }
}