using LudumDare54.Core.States;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LudumDare54.Graphics.Pages.States;

public partial class DeckSelectionStateComponent {
    [Parameter]
    public required DeckSelectionState DeckSelectionState { get; set; }

    [Inject]
    public required AudioPlayer AudioPlayer { get; set; }
    [Inject]
    public required IJSRuntime JSRuntime { get; set; }

    private Guid _innerGuid;
    private Random _random = new();
    private Int32 _randomIdx;

    protected override void OnInitialized() {
        _innerGuid = Guid.NewGuid();
        _randomIdx = _random.Next(0, 1000000);
        base.OnInitialized();
    }

    protected override void OnAfterRender(Boolean firstRender) {
        JSRuntime.InvokeAsync<String>("resizePreview", _innerGuid);

        base.OnAfterRender(firstRender);
    }

    private void OnCardHover() {
        AudioPlayer.Play("_content/LudumDare54.Graphics/audio/cardButtonHover.wav");
    }

    private void OnCardSelect() {
        AudioPlayer.Play("_content/LudumDare54.Graphics/audio/cardButtonSelect.wav");
    }

    private void OnHover() {
        AudioPlayer.Play("_content/LudumDare54.Graphics/audio/menuButtonHover.wav");
    }

    private void OnSelect() {
        AudioPlayer.Play("_content/LudumDare54.Graphics/audio/menuButtonSelect.wav");
    }
}
