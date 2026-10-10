namespace Loupedeck.DockerPlugin.Helpers;

public static class ActionIcons
{
    public const String Start = "play-solid-full.svg";
    public const String Stop = "stop-solid-full.svg";
    public const String Container = "container.svg";
    public const String Stack = "stack.svg";

    public static String ForToggle(ToggleAction action, String unknownStateIcon) => action switch
    {
        ToggleAction.Start => Start,
        ToggleAction.Stop => Stop,
        _ => unknownStateIcon,
    };
}
