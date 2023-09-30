using LudumDare54.Core.Tags;

namespace LudumDare54.Core.Scenes;

public class StaticSelectingAnchor : Anchor {
    public Single PositionX { get; set; }
    public Single PositionY { get; set; }
    public Single Scale { get; set; } = 1;
    public Int32 Chance { get; set; } = 100;
    public List<Tag> Tags { get; set; } = new();
}
