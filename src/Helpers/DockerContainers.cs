namespace Loupedeck.DockerPlugin.Helpers;

using System.Text.Json.Serialization;

using Types;

public class DockerContainers
{
    [JsonPropertyName("")]
    public List<DockerContainer> Containers { get; set; }
}