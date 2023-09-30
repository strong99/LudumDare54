namespace LudumDare54.Graphics;

public interface SessionSettings {
    Single MasterVolume { get; set; }
    Single SoundVolume { get; set; }
    Single MusicVolume { get; set; }
    Task AwaitLoadingComplete { get; }
}
