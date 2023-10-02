using LudumDare54.Core;
using LudumDare54.Core.Scenes;
using LudumDare54.Core.States;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LudumDare54.Graphics.Razor.Pages.States;

public partial class CardResultStateComponent {
    [Parameter]
    public required CardResultState CardResultState { get; set; }

    [Parameter]
    public required CardResultState State { get; set; }

    [Parameter]
    public required Session Session { get; set; }

    [Inject]
    public required AudioPlayer AudioPlayer { get; set; }
    [Inject]
    public required IJSRuntime JSRuntime { get; set; }

    private Guid _innerGuid;
    private readonly Random _random = new();

    [Parameter]
    public required SceneComposer SceneManager { get; set; }

    protected override void OnInitialized() {
        _innerGuid = Guid.NewGuid();
        base.OnInitialized();
    }

    protected override void OnParametersSet() {
        OnPlayResult();

        SceneManager.GenerateAltScene(Session.AllActiveTags);

        base.OnParametersSet();
    }

    protected override void OnAfterRender(Boolean firstRender) {
        JSRuntime.InvokeAsync<String>("resizePreview", _innerGuid);

        base.OnAfterRender(firstRender);
    }

    private void OnHover() {
        AudioPlayer.Play("_content/LudumDare54.Graphics.Razor/audio/menuButtonHover.ogg");
    }

    private void OnSelect() {
        AudioPlayer.Play("_content/LudumDare54.Graphics.Razor/audio/menuButtonSelect.ogg");
    }

    private void OnPlayResult() {
        AudioPlayer.Play($"_content/LudumDare54.Graphics.Razor/audio/{(State.Success ? "success" : "failure")}.ogg", true);
    }
}
