namespace LudumDare54.Core.Scenes;

public class StaticAnchor : Anchor {
    public Single PositionX { get; set; }
    public Single PositionY { get; set; }
    public Single Scale { get; set; }
    public Int32 Chance { get; set; } = 100;
    public String ImageFile { get; set; }
}
