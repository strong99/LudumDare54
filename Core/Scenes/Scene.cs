using LudumDare54.Core.Tags;

namespace LudumDare54.Core.Scenes;

public class Scene {
    public required String Id { get; set; }
    public List<Anchor> Anchors { get; set; } = new();
    public List<Tag> Tags { get; set; } = new();
}
