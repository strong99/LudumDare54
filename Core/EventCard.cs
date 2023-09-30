using LudumDare54.Core.Conditions;
using System.Diagnostics.CodeAnalysis;

namespace LudumDare54.Core;

public class EventCard {
    public required String Id { get; set; }
    public List<Outcome> Outcome { get; set; } = new();
    public List<Condition> Conditions { get; set; } = new();
    public List<NextEventCard> Next { get; set; } = new();
    public OutcomeResult Apply(ResourceCard choice, Session session, StateManager stateManager) {
        foreach (var outcome in Outcome) {
            if (outcome.Can(choice, session)) {
                outcome.Apply(choice, session, stateManager);
                return new OutcomeResult(
                    outcome.Id,
                    outcome.Success
                );
            }
        }
        throw new Exception("No default outcome state was found");
    }

    public EventCard() { }

    [SetsRequiredMembers]
    public EventCard(String id, IEnumerable<Outcome>? outcomes = null) {
        Id = id;

        if (outcomes is not null) {
            Outcome = outcomes.ToList();
        }
    }
}
