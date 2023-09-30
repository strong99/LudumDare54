using LudumDare54.Graphics;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.X509Certificates;

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

    public void Continue() {
        SessionManager.GetOrCreate();
        NavigationManager.NavigateTo("/play");
    }

    public void StartNew() {
        SessionManager.NewSession();
        NavigationManager.NavigateTo("/play");
    }
}
