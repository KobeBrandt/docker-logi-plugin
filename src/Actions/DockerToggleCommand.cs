namespace Loupedeck.DockerPlugin;

using Helpers;

using Types;

public abstract class DockerToggleCommand : PluginDynamicCommand
{
    private const String CachedParameterNamesSettingFormat = "CachedParameterNames.{0}";

    private readonly String _parameterGroupName;
    private readonly String _unknownStateIcon;
    private readonly ParameterNamesTracker _parameterNames = new();

    protected DockerToggleCommand(String displayName, String description, String unknownStateIcon)
    {
        this.DisplayName = displayName;
        this.Description = description;
        this._parameterGroupName = displayName;
        this._unknownStateIcon = unknownStateIcon;
    }

    protected abstract IEnumerable<String> GetParameterNames(List<DockerContainer> containers);

    protected abstract ToggleAction GetToggleAction(String parameterName);

    protected abstract Boolean Toggle(String parameterName);

    protected override Boolean OnLoad()
    {
        DockerServices.StateMonitor.StatesChanged += this.OnContainerStatesChanged;
        this.LoadCachedParameterNames();
        DockerServices.StateMonitor.RefreshIfNeverLoaded(DockerConstants.ActionLoadRefreshTimeout);
        this.SyncParameters();
        return base.OnLoad();
    }

    protected override Boolean OnUnload()
    {
        DockerServices.StateMonitor.StatesChanged -= this.OnContainerStatesChanged;
        return base.OnUnload();
    }

    protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
    {
        var toggleAction = actionParameter == null ? ToggleAction.Unknown : this.GetToggleAction(actionParameter);
        return BitmapHelper.MakeBitmapImage(ActionIcons.ForToggle(toggleAction, this._unknownStateIcon));
    }

    protected override void RunCommand(String actionParameter)
    {
        if (actionParameter == null || !DockerServices.StatusReporter.EnsureDockerReady())
        {
            return;
        }

        if (!this.Toggle(actionParameter))
        {
            PluginLog.Warning($"Toggling {actionParameter} failed");
        }

        DockerServices.StateMonitor.Refresh();
    }

    private void OnContainerStatesChanged(Object sender, EventArgs e)
    {
        this.SyncParameters();
        this.ActionImageChanged();
    }

    private String CachedParameterNamesSetting => String.Format(CachedParameterNamesSettingFormat, this._parameterGroupName);

    // Options+ drops actions whose load takes too long, so cached names cover loads where Docker is not reachable yet.
    private void LoadCachedParameterNames()
    {
        if (this.Plugin.TryGetPluginSetting(this.CachedParameterNamesSetting, out var json)
            && ParameterNamesSerializer.TryDeserialize(json, out var names))
        {
            this.ApplyParameterNames(names);
        }
    }

    private void SyncParameters()
    {
        var containers = DockerServices.StateMonitor.Containers;
        if (containers != null && this.ApplyParameterNames(this.GetParameterNames(containers)))
        {
            this.Plugin.SetPluginSetting(this.CachedParameterNamesSetting, ParameterNamesSerializer.Serialize(this._parameterNames.Names));
        }
    }

    private Boolean ApplyParameterNames(IEnumerable<String> names)
    {
        if (!this._parameterNames.UpdateIfChanged(names))
        {
            return false;
        }

        this.RemoveAllParameters();
        foreach (var name in this._parameterNames.Names)
        {
            this.AddParameter(name, name, this._parameterGroupName);
        }

        this.ParametersChanged();
        PluginLog.Info($"{this._parameterGroupName} parameters updated: {this._parameterNames.Names.Count}");
        return true;
    }
}
