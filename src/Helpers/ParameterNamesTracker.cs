namespace Loupedeck.DockerPlugin.Helpers;

public sealed class ParameterNamesTracker
{
    public IReadOnlyList<String> Names { get; private set; } = [];

    public Boolean UpdateIfChanged(IEnumerable<String> names)
    {
        var sortedNames = names.Order(StringComparer.Ordinal).ToList();
        if (sortedNames.SequenceEqual(this.Names))
        {
            return false;
        }

        this.Names = sortedNames;
        return true;
    }
}
