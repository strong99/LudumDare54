using LudumDare54.Graphics;

namespace LudumDare54.Editor;

public class MauiSessionSettings : SessionSettings {
    private readonly AudioPlayer _audioPlayer;

    public Single MasterVolume {
        get => _masterVolume;
        set {
            _masterVolume = value;
            Preferences.Default.Set(nameof(MasterVolume), ((Int32)(value * 100)).ToString());
            _audioPlayer.SetMasterVolume(_masterVolume);
        }
    }
    private Single _masterVolume = 1;
    public Single SoundVolume {
        get => _soundVolume;
        set {
            _soundVolume = value;
            Preferences.Default.Set(nameof(SoundVolume), ((Int32)(value * 100)).ToString());
            _audioPlayer.SetSoundVolume(_soundVolume);
        }
    }
    private Single _soundVolume = 1;
    public Single MusicVolume {
        get => _musicVolume;
        set {
            _musicVolume = value;
            Preferences.Default.Set(nameof(MusicVolume), ((Int32)(value * 100)).ToString());
            _audioPlayer.SetMusicVolume(_musicVolume);
        }
    }
    private Single _musicVolume = 1;

    public Task AwaitLoadingComplete { get; } = Task.CompletedTask;

    public MauiSessionSettings(AudioPlayer audioPlayer) {
        _audioPlayer = audioPlayer;

        _masterVolume = Single.Parse(Preferences.Default.Get(nameof(MasterVolume), "75")) / 100.0f;
        _soundVolume = Single.Parse(Preferences.Default.Get(nameof(SoundVolume), "75")) / 100.0f;
        _musicVolume = Single.Parse(Preferences.Default.Get(nameof(MusicVolume), "75")) / 100.0f;

        _audioPlayer.SetMasterVolume(_masterVolume);
        _audioPlayer.SetSoundVolume(_soundVolume);
        _audioPlayer.SetMusicVolume(_musicVolume);
    }
}
