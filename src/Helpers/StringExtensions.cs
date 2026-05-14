namespace Loupedeck.DockerPlugin;

public static class StringExtensions
{
    public static Boolean EqualsNoCase(this String str, String other) =>
        String.Equals(str, other, StringComparison.OrdinalIgnoreCase);
}
