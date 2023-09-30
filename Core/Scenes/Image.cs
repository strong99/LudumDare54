using LudumDare54.Core.Tags;

namespace LudumDare54.Core.Scenes;

public class Image {
    public String File { get; set; }
    public Single OriginX { get; set; }
    public Single OriginY { get; set; }
    public List<Tag> Tags { get; set; }
    public Single Weight { get; set; }
    public List<Anchor> Anchors { get; set; } = new();
}
