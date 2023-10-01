using LudumDare54.Core.Tags;

namespace LudumDare54.Core.Modifiers;

public class AddSessionKeyTagModifier : Modifier {
    public String Key { get; set; }

    public Task Apply(ResourceCard choice, Session session, StateManager stateManager) {
        session.Tags.Add(new KeyTag(Key));
        return Task.CompletedTask;
    }
}
