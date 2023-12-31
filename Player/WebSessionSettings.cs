﻿using LudumDare54.Graphics;
using Microsoft.JSInterop;

namespace LudumDare54.Web;

public class WebSessionSettings : SessionSettings {
    private readonly IJSRuntime _jsRuntime;
    private readonly AudioPlayer _audioPlayer;

    public Single MasterVolume {
        get => _masterVolume;
        set {
            _masterVolume = value;
            _jsRuntime.InvokeVoidAsync("localStorage.setItem", nameof(MasterVolume), (value * 100).ToString());
            _audioPlayer.SetMasterVolume(_masterVolume);
        }
    }
    private Single _masterVolume = 1;

    public Single SoundVolume {
        get => _soundVolume;
        set {
            _soundVolume = value;
            _jsRuntime.InvokeVoidAsync("localStorage.setItem", nameof(SoundVolume), (value * 100).ToString());
            _audioPlayer.SetSoundVolume(_soundVolume);
        }
    }
    private Single _soundVolume = 1;

    public Single MusicVolume {
        get => _musicVolume;
        set {
            _musicVolume = value;
            _jsRuntime.InvokeVoidAsync("localStorage.setItem", nameof(MusicVolume), (value * 100).ToString());
            _audioPlayer.SetMusicVolume(_musicVolume);
        }
    }
    private Single _musicVolume = 1;

    public Task AwaitLoadingComplete { get; }

    public WebSessionSettings(IJSRuntime jsRuntime, AudioPlayer audioPlayer) {
        _jsRuntime = jsRuntime;
        _audioPlayer = audioPlayer;

        AwaitLoadingComplete = Task.Run(async () => {
            _masterVolume = Int32.Parse(await _jsRuntime.InvokeAsync<String>("localStorage.getItem", nameof(MasterVolume)) ?? "100") / 100.0f;
            _soundVolume = Int32.Parse(await _jsRuntime.InvokeAsync<String>("localStorage.getItem", nameof(SoundVolume)) ?? "100") / 100.0f;
            _musicVolume = Int32.Parse(await _jsRuntime.InvokeAsync<String>("localStorage.getItem", nameof(MusicVolume)) ?? "100") / 100.0f;
        });
    }
}
