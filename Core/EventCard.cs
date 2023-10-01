using LudumDare54.Core.Conditions;
using LudumDare54.Core.Tags;
using System.Diagnostics.CodeAnalysis;

namespace LudumDare54.Core;

public class EventCard {
    public required String Id { get; set; }
    public List<Outcome> Outcome { get; set; } = new();
    public List<Condition> Conditions { get; set; } = new();
    public List<NextEventCard> Next { get; set; } = new();
    public List<Tag> Tags { get; set; } = new();
    public Single Weight { get; set; } = 1;

    public OutcomeResult Apply(ResourceCard choice, Session session, StateManager stateManager) {
        var outcomes = new Dictionary<Single, Outcome>();
        var score = 0.0f;
        var minScore = 0;
        foreach (var outcome in Outcome) {
            if (outcome.Can(choice, session)) {
                score += outcome.Weight;
                outcomes.Add(outcome.Weight == 0 ? --minScore : score, outcome);
            }
        }
        outcomes = outcomes.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value);

        Outcome chosenOutcome;
        if (outcomes.Count == 0) {
            throw new Exception("No outcomes available");
        }
        else if (outcomes.Count == 1) {
            chosenOutcome = outcomes.First().Value;
        }
        else {
            var random = new Random();
            var idx = random.NextDouble() * score;
            chosenOutcome = outcomes.Last(p => idx < p.Key).Value;
        }

        chosenOutcome.Apply(choice, session, stateManager);
        return new OutcomeResult(
            chosenOutcome.Id,
            chosenOutcome.Success,
            chosenOutcome.End
        );
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
