namespace LudumDare54.Core.Modifiers;

public class RemoveKeyTagModifier : Modifier {
    public String Key { get; set; }

    public Task Apply(ResourceCard choice, Session session, StateManager stateManager) {
        session.Tags.RemoveAll(p=>p.Key.Equals(Key, StringComparison.OrdinalIgnoreCase));
        return Task.CompletedTask;
    }
}