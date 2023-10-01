using LudumDare54.Core.Tags;

namespace LudumDare54.Core.Modifiers;

public class ReplaceSessionKeyTagModifier : Modifier {
    public String RemoveKey { get; set; }
    public String AddKey { get; set; }

    public Task Apply(ResourceCard choice, Session session, StateManager stateManager) {
        session.Tags.RemoveAll(p => p.Key.Equals(RemoveKey, StringComparison.OrdinalIgnoreCase));
        session.Tags.Add(new KeyTag(AddKey));
        return Task.CompletedTask;
    }
}
