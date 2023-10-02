namespace LudumDare54.Graphics.Razor;

public interface SessionSettings {
    Single MasterVolume { get; set; }
    Single SoundVolume { get; set; }
    Single MusicVolume { get; set; }
    Task AwaitLoadingComplete { get; }
}
