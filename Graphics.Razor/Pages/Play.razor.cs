﻿using LudumDare54.Core;
using LudumDare54.Core.Scenes;
using LudumDare54.Core.States;
using Microsoft.AspNetCore.Components;

namespace LudumDare54.Graphics.Razor.Pages;

public partial class Play {
    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    [Inject]
    public required RepositoryFactory RepositoryFactory { get; set; }

    [Inject]
    public required SessionManager SessionManager { get; set; }

    [Inject]
    public required AudioPlayer AudioPlayer { get; set; }

    [Inject]
    public required SceneComposerFactory SceneComposerFactory { get; set; }

    private readonly Random _random = new();
    
    private State? _state;

    private SceneComposer _sceneComposer = default!;
    private Repository _repository = default!;
    private Session _session = default!;
    private StateManager _stateManager = default!;

    protected override void OnInitialized() {
        AudioPlayer.Load("_content/LudumDare54.Graphics.Razor/audio/cardButtonHover.ogg");
        AudioPlayer.Load("_content/LudumDare54.Graphics.Razor/audio/cardButtonSelect.ogg");
        PlayMusic();

        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync() {
        _repository = await RepositoryFactory.GetRepository();

        _session = SessionManager.GetOrCreate();
        _stateManager = new() { InputCallback = StateChanged };
        _sceneComposer = await SceneComposerFactory.Create();

        _ = _session.Play(_repository, _stateManager).ContinueWith(t => {
            _state = new ManualState("credits", "Created for Ludum Dare 54 by Strong99 in 48 hours from Scratch, 2023/09/30 - 2023/10/01", () => {
                NavigationManager.NavigateTo("");
                StateHasChanged();
            });
            InvokeAsync(() => StateHasChanged());
        });

        await base.OnParametersSetAsync();
    }

    private void StateChanged(State state) {
        if (state is DeckSelectionState deckSelectionState) {
            _state = new ManualState("intro01", "The frontlines are hard. An ambush criples the defensive lines. You need to warn the city, now!.", () => {
                _state = state;
                StateHasChanged();
            });
        }
        else {
            _state = state;
        }
        StateHasChanged();
    }

    public void PlayMusic() {
        AudioPlayer.PlayMusic("_content/LudumDare54.Graphics.Razor/audio/background.ogg");
    }

    public void OnHover() {
        AudioPlayer.Play("_content/LudumDare54.Graphics.Razor/audio/menuButtonHover.ogg");
    }

    public void OnActivate() {
        AudioPlayer.Play("_content/LudumDare54.Graphics.Razor/audio/menuButtonSelect.ogg");
    }
}

public class ManualState : State {
    public String Id { get; }
    public String Text { get; }
    private Action _action;

    public ManualState(String id, String text, Action action) {
        Id = id;
        Text = text;
        _action = action;
    }

    public void Finish() {
        _action.Invoke();
    }
}