namespace LudumDare54.Graphics;

public interface SessionSettings {
    Boolean Audio { get; set; }
    Task AwaitLoadingComplete { get; }
}