namespace Loupedeck.DockerPlugin.Helpers;

using System.Text.Json;

public static class ParameterNamesSerializer
{
    public static String Serialize(IEnumerable<String> names) => JsonSerializer.Serialize(names);

    public static Boolean TryDeserialize(String json, out List<String> names)
    {
        names = null;
        if (String.IsNullOrEmpty(json))
        {
            return false;
        }

        try
        {
            names = JsonSerializer.Deserialize<List<String>>(json);
            return names != null;
        }
        catch (JsonException ex)
        {
            PluginLog.Warning(ex, "Cached parameter names are not valid JSON");
            return false;
        }
    }
}
