using LudumDare54.Graphics;
using Microsoft.AspNetCore.Components;

namespace LudumDare54.Graphics.Pages;

public partial class Index {
    [Inject]
    public RepositoryFactory RepositoryFactory { get; set; }

    [Inject]
    public SessionSettings SessionSettings { get; set; }

    public String Preset { get => (RepositoryFactory as LoadableRepositoryFactory)?.Preset; set => (RepositoryFactory as LoadableRepositoryFactory).Preset = value; }

    public Boolean CanEditRepository { get => RepositoryFactory is WriteableRepositoryFactory; }
    public Boolean CanSelectRepository { get => RepositoryFactory is LoadableRepositoryFactory; }

    public Task Select() {
        if (RepositoryFactory is LoadableRepositoryFactory loadableRepositoryFactory) {
            return loadableRepositoryFactory.SelectFile();
        }
        return Task.CompletedTask;
    }
}
