namespace Loupedeck.DockerPlugin.Types;

using System.Text.Json.Serialization;

public class DockerService
{
    [JsonPropertyName("ID")] public String Id { get; set; }

    [JsonPropertyName("Spec")] public DockerServiceSpec Spec { get; set; }
}

public class DockerServiceSpec
{
    [JsonPropertyName("Name")] public String Name { get; set; }
}