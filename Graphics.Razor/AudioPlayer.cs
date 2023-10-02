using Microsoft.JSInterop;

namespace LudumDare54.Graphics.Razor;

public class AudioPlayer {
    private readonly IJSRuntime _jsRuntime;

    public AudioPlayer(IJSRuntime jsRuntime/*, SessionSettings sessionSettings*/) {
        _jsRuntime = jsRuntime;
        /*_sessionSettings = sessionSettings;*/
    }

    public ValueTask Load(String id)
        => _jsRuntime.InvokeVoidAsync("loadSound", id);

    public ValueTask Play(String id, Boolean overrideMusic = false)
        => _jsRuntime.InvokeVoidAsync("playSound", id, overrideMusic);

    public ValueTask PlayMusic(String id)
        => _jsRuntime.InvokeVoidAsync("playMusic", id);

    public ValueTask StopMusic()
        => _jsRuntime.InvokeVoidAsync("stopMusic");
    public ValueTask SetMasterVolume(Single volume)
        => _jsRuntime.InvokeVoidAsync("setAudioVolume", volume);
    public ValueTask SetMusicVolume(Single volume)
        => _jsRuntime.InvokeVoidAsync("setMusicVolume", volume);
    public ValueTask SetSoundVolume(Single volume)
        => _jsRuntime.InvokeVoidAsync("setSoundVolume", volume);

    private Boolean _first = true;
    private async ValueTask Invoke(String method, Object[]? parameters = null) {
        await _jsRuntime.InvokeVoidAsync("setSoundMusic", parameters);
    }
}