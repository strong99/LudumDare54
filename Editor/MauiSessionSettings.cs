using LudumDare54.Graphics;

namespace LudumDare54.Editor;

public class MauiSessionSettings : SessionSettings {
    public Boolean Audio {
        get => _audio;
        set {
            _audio = value;
            Preferences.Default.Set("audioEnabled", value.ToString());
        }
    }
    private Boolean _audio = true;

    public Task AwaitLoadingComplete { get; } = Task.CompletedTask;

    public MauiSessionSettings() {
        Audio = Boolean.Parse(Preferences.Default.Get("audioEnabled", "true"));
    }
}
