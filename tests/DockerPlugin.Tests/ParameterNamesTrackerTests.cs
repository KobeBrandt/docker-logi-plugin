namespace Loupedeck.DockerPlugin.Tests;

using Helpers;

using Xunit;

public class ParameterNamesTrackerTests
{
    private readonly ParameterNamesTracker _tracker = new();

    [Fact]
    public void UpdateIfChanged_NewNames_ReturnsTrueAndStoresSorted()
    {
        Assert.True(this._tracker.UpdateIfChanged(["b", "a"]));
        Assert.Equal(["a", "b"], this._tracker.Names);
    }

    [Fact]
    public void UpdateIfChanged_SameNamesInOtherOrder_ReturnsFalse()
    {
        this._tracker.UpdateIfChanged(["a", "b"]);

        Assert.False(this._tracker.UpdateIfChanged(["b", "a"]));
    }

    [Fact]
    public void UpdateIfChanged_NoNamesInitially_ReturnsFalse() => Assert.False(this._tracker.UpdateIfChanged([]));
}
