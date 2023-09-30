using LudumDare54.Core.Conditions;
using LudumDare54.Core.Modifiers;

namespace LudumDare54.Core;

public class Outcome {
    public String Id { get; set; }
    public Single Weight { get; set; } = 1;

    /// <summary>
    /// A hint whether this outcome should be seen as successful or not.
    /// </summary>
    public Boolean Success { get; set; } = true;

    public List<Condition> Conditions { get; set; } = new();
    public List<Modifier> Modifiers { get; set; } = new();
    public List<NextEventCard> Next { get; set; } = new();

    public Boolean Can(ResourceCard choice, Session session) {
        foreach (var condition in Conditions) {
            if (!condition.IsMet(choice, session)) {
                return false;
            }
        }

        return true;
    }

    public void Apply(ResourceCard choice, Session session, StateManager stateManager) {
        foreach (var modifier in Modifiers) {
            modifier.Apply(choice, session, stateManager);
        }
    }
}
