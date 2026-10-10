namespace Loupedeck.DockerPlugin.Tests;

using Helpers;

using Xunit;

public class ActionIconsTests
{
    [Theory]
    [InlineData(ToggleAction.Start, ActionIcons.Start)]
    [InlineData(ToggleAction.Stop, ActionIcons.Stop)]
    [InlineData(ToggleAction.Unknown, ActionIcons.Container)]
    public void ForToggle_ReturnsIconMatchingAction(ToggleAction action, String expectedIcon) =>
        Assert.Equal(expectedIcon, ActionIcons.ForToggle(action, ActionIcons.Container));
}
