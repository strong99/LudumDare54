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
    void Save();
}