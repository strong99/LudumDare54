namespace LudumDare54.Core.Modifiers;

public interface Modifier {
    Task Apply(ResourceCard choice, Session session, StateManager stateManager);
}
