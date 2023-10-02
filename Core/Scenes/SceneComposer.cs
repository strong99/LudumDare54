using LudumDare54.Core.Tags;

namespace LudumDare54.Core.Scenes;

public interface SceneComposer {
    IEnumerable<ImageToRender>? SceneElements { get; }

    IEnumerable<ImageToRender> GenerateNewScene(IEnumerable<Tag> tags);
    IEnumerable<ImageToRender> GenerateAltScene(IEnumerable<Tag> tags);
}
