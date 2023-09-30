using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LudumDare54.Graphics.Pages;

public partial class Index {
    [Inject]
    public required RepositoryFactory RepositoryFactory { get; set; }

    [Inject]
    public required SessionSettings SessionSettings { get; set; }

    [Inject]
    public required SessionManager SessionManager { get; set; }

    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    [Inject]
    public required AudioPlayer AudioPlayer { get; set; }

    [Inject]
    public required IServiceProvider ServiceProvider {
        get => _serviceProvider;
        set {
            _serviceProvider = value;
            _quitApplicationFeature = _serviceProvider.GetService<QuitApplicationFeature>();
        }
    }
    private IServiceProvider _serviceProvider;

    public QuitApplicationFeature? _quitApplicationFeature;

    public void TryQuit() {
        OnActivate();
        _quitApplicationFeature?.TryQuit();
    }

    public Boolean CanQuit { get => _quitApplicationFeature is not null; }

    public String Preset { get => (RepositoryFactory as LoadableRepositoryFactory)?.Preset; set => (RepositoryFactory as LoadableRepositoryFactory).Preset = value; }

    public Boolean CanEditRepository { get => RepositoryFactory is WriteableRepositoryFactory; }
    public Boolean CanSelectRepository { get => RepositoryFactory is LoadableRepositoryFactory; }

    public Task Select() {
        if (RepositoryFactory is LoadableRepositoryFactory loadableRepositoryFactory) {
            return loadableRepositoryFactory.SelectFile();
        }
        return Task.CompletedTask;
    }

    protected override void OnInitialized() {
        AudioPlayer.Load("_content/LudumDare54.Graphics/audio/menuButtonHover.wav");
        AudioPlayer.Load("_content/LudumDare54.Graphics/audio/menuButtonSelect.wav");
        PlayMusic();

        base.OnInitialized();
    }

    public async Task Continue() {
        OnActivate();
        SessionManager.GetOrCreate();
        NavigationManager.NavigateTo("/play");
    }

    public void StartNew() {
        OnActivate();
        SessionManager.NewSession();
        NavigationManager.NavigateTo("/play");
    }

    public void OnHover() {
        AudioPlayer.Play("_content/LudumDare54.Graphics/audio/menuButtonHover.wav");
    }

    public void OnActivate() {
        AudioPlayer.Play("_content/LudumDare54.Graphics/audio/menuButtonSelect.wav");
    }

    public void PlayMusic() {
        AudioPlayer.PlayMusic("_content/LudumDare54.Graphics/audio/background.wav");
    }
}
