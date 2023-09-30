using LudumDare54.Graphics;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LudumDare54.Web;

public class WebSessionSettings : SessionSettings {
    [Inject]
    public required IJSRuntime JSRuntime { get; init; }

    private const String AudioEnabledKey = "audioEnabled";

    public Boolean Audio {
        get => _audio;
        set {
            _audio = value;
            JSRuntime.InvokeVoidAsync("localStorage.setItem", AudioEnabledKey, value.ToString());
        }
    }
    private Boolean _audio = true;

    public Task AwaitLoadingComplete { get; }

    public WebSessionSettings() {
        AwaitLoadingComplete = Task.Run(async () => {
            Audio = Boolean.Parse(
                (await JSRuntime.InvokeAsync<String>("localStorage.getItem", AudioEnabledKey)) ?? "true"
            );
        });
    }
}
