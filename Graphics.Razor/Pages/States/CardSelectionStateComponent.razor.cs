using LudumDare54.Core;
using LudumDare54.Core.Scenes;
using LudumDare54.Core.States;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LudumDare54.Graphics.Razor.Pages.States;

public partial class CardSelectionStateComponent {
    [Parameter]
    public required CardSelectionState CardSelectionState { get; set; }

    [Inject]
    public required AudioPlayer AudioPlayer { get; set; }

    [Inject]
    public required IJSRuntime JSRuntime { get; set; }

    [Parameter]
    public required SceneComposer SceneManager { get; set; }

    private Guid _innerGuid;
    private Random _random = new();

    protected override void OnInitialized() {
        _innerGuid = Guid.NewGuid();
        base.OnInitialized();
    }

    protected override void OnAfterRender(Boolean firstRender) {
        JSRuntime.InvokeAsync<String>("resizePreview", _innerGuid);

        base.OnAfterRender(firstRender);
    }


    [Parameter]
    public required CardSelectionState State { get; set; }

    [Parameter]
    public required Session Session { get; set; }

    [Inject]
    public required RepositoryFactory RepositoryFactory { get; set; }

    private Repository _repository = default!;

    protected void Toggle(String eventCardId) {
        Session.Deck ??= new();
        if (Session.Deck.Contains(eventCardId)) {
            Session.Deck.Remove(eventCardId);
        }
        else {
            Session.Deck.Add(eventCardId);
        }
    }

    protected override async Task OnParametersSetAsync() {
        _repository = await RepositoryFactory.GetRepository();

        SceneManager.GenerateNewScene(Session.AllActiveTags);

        await base.OnParametersSetAsync();
    }

    private void OnCardHover() {
        AudioPlayer.Play("_content/LudumDare54.Graphics.Razor/audio/cardButtonHover.ogg");
    }

    private void OnCardSelect() {
        AudioPlayer.Play("_content/LudumDare54.Graphics.Razor/audio/cardButtonSelect.ogg");
    }
}
