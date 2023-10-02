using LudumDare54.Core;
using LudumDare54.Core.Scenes;

namespace LudumDare54.Graphics.Razor;

public interface RepositoryFactory {
    Task<Repository> GetRepository();
}

public interface LoadableRepositoryFactory {
    String Preset { get; set; }
    Task SelectFile();
}

public interface WriteableRepositoryFactory : LoadableRepositoryFactory {
    Task AddImage();
    void Save();
}

public class SessionManager {
    public Session? Session { get; set; }

    public Boolean HasSession { get => Session is not null; }

    public Session GetOrCreate() => Session ??= new();
    public Session NewSession() => Session = new();
}

public interface QuitApplicationFeature {
    void TryQuit();
}

public interface SceneComposerFactory {
    Task<SceneComposer> Create();
}

public class SceneManagerFactory<TType> : SceneComposerFactory where TType : class, SceneComposer {
    private readonly RepositoryFactory _repositoryFactory;
    public SceneManagerFactory(RepositoryFactory repositoryFactory) {
        _repositoryFactory = repositoryFactory;
    }

    public async Task<SceneComposer> Create() {
        var repository = await _repositoryFactory.GetRepository();
        return (TType)Activator.CreateInstance(typeof(TType), repository)!;
    }
}