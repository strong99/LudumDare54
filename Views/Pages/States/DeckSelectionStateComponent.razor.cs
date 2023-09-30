using LudumDare54.Core;
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

    [Parameter]
    public required DeckSelectionState State { get; set; }

    [Parameter]
    public required Session Session { get; set; }

    [Inject]
    public required RepositoryFactory RepositoryFactory { get; set; }

    private Repository _repository = default!;

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
        AudioPlayer.Play("_content/LudumDare54.Graphics/audio/cardButtonHover.ogg");
    }

    private void OnCardSelect() {
        AudioPlayer.Play("_content/LudumDare54.Graphics/audio/cardButtonSelect.ogg");
    }

    private void OnHover() {
        AudioPlayer.Play("_content/LudumDare54.Graphics/audio/menuButtonHover.ogg");
    }

    private void OnSelect() {
        AudioPlayer.Play("_content/LudumDare54.Graphics/audio/menuButtonSelect.ogg");
    }

    protected void Toggle(ResourceCard card) {
        OnCardSelect();

        if (State.Deck.Contains(card)) {
            State.Deck.Remove(card);
        }
        else if ((State.Deck?.Count ?? 0) >= 10) {
            return;
        }
        else {
            State.Deck.Add(card);
        }
    }

    protected override async Task OnParametersSetAsync() {
        _repository = await RepositoryFactory.GetRepository();

        await base.OnParametersSetAsync();
    }
}
