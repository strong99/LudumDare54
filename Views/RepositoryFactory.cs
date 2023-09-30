using LudumDare54.Core;

namespace LudumDare54.Graphics;

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
