using LudumDare54.Core;
using Microsoft.AspNetCore.Components;

namespace LudumDare54.Graphics.Razor.Pages;

public partial class Settings {
    [Inject]
    public required RepositoryFactory RepositoryFactory { get; set; }

    [Inject]
    public required SessionSettings SessionSettings { get; set; }

    [Inject]
    public required SessionManager SessionManager { get; set; }

    [Inject]
    public required AudioPlayer AudioPlayer { get; set; }

    private Repository _repository = default!;

    public String Preset { get => (RepositoryFactory as LoadableRepositoryFactory)?.Preset; set => (RepositoryFactory as LoadableRepositoryFactory).Preset = value; }

    public Boolean CanEditRepository { get => RepositoryFactory is WriteableRepositoryFactory; }
    public Boolean CanSelectRepository { get => RepositoryFactory is LoadableRepositoryFactory; }

    public Task Select() {
        if (RepositoryFactory is LoadableRepositoryFactory loadableRepositoryFactory) {
            return loadableRepositoryFactory.SelectFile();
        }
        return Task.CompletedTask;
    }

    protected override async Task OnParametersSetAsync() {
        _repository = await RepositoryFactory.GetRepository();

        await base.OnParametersSetAsync();
    }

    public void OnHover() {
        AudioPlayer.Play("_content/LudumDare54.Graphics.Razor/audio/menuButtonHover.ogg");
    }

    public void OnActivate() {
        AudioPlayer.Play("_content/LudumDare54.Graphics.Razor/audio/menuButtonSelect.ogg");
    }
}
