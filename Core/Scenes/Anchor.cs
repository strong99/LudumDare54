namespace LudumDare54.Core.Scenes;

public interface Anchor {
    Single PositionX { get; }
    Single PositionY { get; }
    Int32 Chance { get; }
    Single Scale { get; }
}
