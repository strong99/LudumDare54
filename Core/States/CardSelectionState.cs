namespace LudumDare54.Core.States;

public class CardSelectionState : State {
    public static StateTask<CardSelectionState> Create(EventCard eventCard, IEnumerable<ResourceCard> deck) {
        var taskCompletionSource = new TaskCompletionSource<CardSelectionState>();
        return new StateTask<CardSelectionState>(
            new CardSelectionState(eventCard, deck, taskCompletionSource),
            taskCompletionSource.Task
        );
    }

    public EventCard EventCard { get; set; }
    public List<ResourceCard> Deck { get; private set; }
    public ResourceCard? Choice { get; set; }

    private TaskCompletionSource<CardSelectionState> _taskCompletionSource;

    public CardSelectionState(EventCard eventCard, IEnumerable<ResourceCard> deck, TaskCompletionSource<CardSelectionState> taskCompletionSource) {
        _taskCompletionSource = taskCompletionSource;
        EventCard = eventCard;
        Deck = deck.ToList();
    }

    public void Finish(ResourceCard? response = null) {
        if (_taskCompletionSource.Task.IsCompleted) {
            throw new Exception("This state has already been finished");
        }

        Choice ??= response;

        _taskCompletionSource.SetResult(this);
    }
}
