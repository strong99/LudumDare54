using LudumDare54.Core.States;
using LudumDare54.Core.Tags;

namespace LudumDare54.Core;

public class Session {
    public List<String>? Deck { get; set; }
    public EventCard? EventCard { get; set; }
    public ResourceCard? Choice { get; set; }
    public OutcomeResult? Outcome { get; set; }
    public Boolean Active { get; set; } = true;
    public List<Tag> Tags { get; } = new();

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
        if (Deck is null) {
            var deckState = DeckSelectionState.Create(10);
            stateManager.Inform(deckState.State);
            await deckState.Task;

            Deck = deckState.State.Deck.Select(p => p.Id).ToList();
        }

        if (EventCard is null) {
            EventCard = repository.EventCards[_random.Next(repository.EventCards.Count)];
            Choice = null;
        }

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

        if (!Outcome.Success) {
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
