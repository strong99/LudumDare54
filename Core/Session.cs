using LudumDare54.Core.States;
using LudumDare54.Core.Tags;

namespace LudumDare54.Core;

public class Session {
    public List<String>? Deck { get; set; }
    public EventCard? EventCard { get; set; }
    public Boolean Active { get; set; } = true;
    public List<Tag> Tags { get; } = new();

    private Random _random = new();

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

            Deck = deckState.State.Deck.Select(p=>p.Id).ToList();
        }

        EventCard = repository.EventCards[_random.Next(repository.EventCards.Count)];

        var eventState = CardSelectionState.Create(EventCard, repository.ResourceCards.Where(x=>Deck.Contains(x.Id)).ToList());
        stateManager.Inform(eventState.State);
        await eventState.Task;

        var choice = eventState.State.Choice;
        if (choice is null) {
            throw new Exception("Player should have made a choice before round finish. Something went wrong in the process");
        }

        var result = EventCard.Apply(choice, this, stateManager);

        var resultState = CardResultState.Create(result.Id, result.Success);
        stateManager.Inform(resultState.State);
        await resultState.Task;
    }
}
