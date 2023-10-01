using LudumDare54.Core.States;
using LudumDare54.Core.Tags;
using System.ComponentModel.DataAnnotations;

namespace LudumDare54.Core;

public class Session {
    public List<String>? Deck { get; set; }
    public EventCard? EventCard { get; set; }
    public ResourceCard? Choice { get; set; }
    public OutcomeResult? Outcome { get; set; }
    public Boolean Active { get; set; } = true;
    public List<Tag> Tags { get; } = new();
    public List<Tag> AllActiveTags { get; } = new();

    private Random _random = new();

    public Int32 Round { get; set; } = 0;

    public async Task Play(Repository repository, StateManager stateManager) {

        if (repository.EventCards.Count == 0) {
            throw new Exception("No event cards in repository");
        }

        if (repository.ResourceCards.Count == 0) {
            throw new Exception("No resource cards in repository");
        }

        while (Active) {
            await Progress(repository, stateManager);
        }
    }

    private async Task Progress(Repository repository, StateManager stateManager) {
        AllActiveTags.Clear();
        AllActiveTags.AddRange(Tags);

        if (Deck is null) {
            var deckState = DeckSelectionState.Create(10);
            stateManager.Inform(deckState.State);
            await deckState.Task;

            Deck = deckState.State.Deck.Select(p => p.Id).ToList();
        }

        if (EventCard is null) {
            var availableCards = repository.EventCards.Where(c=>c.Conditions.All(x=>x.IsMet(null, this))).ToList();
            if (availableCards.Count == 0) {
                Active = false;
                return;
            }

            var maxScore = 0.0f;
            var weightedCards = new Dictionary<Single, EventCard>();
            foreach(var card in availableCards) {
                if (card.Weight <= 0) {
                    continue;
                }
                maxScore += card.Weight;
                weightedCards.Add(maxScore, card);
            }

            var idx =_random.NextDouble() * maxScore;
            EventCard = weightedCards.First(x => idx < x.Key).Value;

            Choice = null;
        }
        AllActiveTags.AddRange(EventCard.Tags);

        if (Choice is null) {
            var eventState = CardSelectionState.Create(EventCard, repository.ResourceCards.Where(x => Deck.Contains(x.Id)).ToList());
            stateManager.Inform(eventState.State);
            await eventState.Task;

            Choice = eventState.State.Choice;
            if (Choice is null) {
                throw new Exception("Player should have made a choice before round finish. Something went wrong in the process");
            }
        }

        if (Outcome is null) {
            Outcome = EventCard.Apply(Choice, this, stateManager);
        }

        var resultState = CardResultState.Create(Outcome.Id, Outcome.Success);
        stateManager.Inform(resultState.State);
        await resultState.Task;

        if (Outcome.End) {
            Active = false;
            Deck = null;
            Tags.Clear();
        }

        Outcome = null;
        Choice = null;
        EventCard = null;

        ++Round;
    }
}
