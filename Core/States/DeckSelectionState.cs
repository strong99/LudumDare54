namespace LudumDare54.Core.States;

public class DeckSelectionState : State {
    public static StateTask<DeckSelectionState> Create(Int32 amount) {
        var taskCompletionSource = new TaskCompletionSource<DeckSelectionState>();
        return new StateTask<DeckSelectionState>(
            new DeckSelectionState(amount, taskCompletionSource),
            taskCompletionSource.Task
        );
    }

    public Int32 Amount { get; private set; }
    public List<ResourceCard> Deck { get; private set; } = new();

    private readonly TaskCompletionSource<DeckSelectionState> _taskCompletionSource;

    public DeckSelectionState(Int32 amount, TaskCompletionSource<DeckSelectionState> taskCompletionSource) {
        _taskCompletionSource = taskCompletionSource;
        Amount = amount;
    }

    public void Finish() {
        if (_taskCompletionSource.Task.IsCompleted) {
            throw new Exception("This state has already been finished");
        }
        else if (Amount < Deck.Count || !Deck.Any()) {
            throw new Exception($"Too many cards {Deck.Count} selected. Max {Amount} cards expected.");
        }

        _taskCompletionSource.SetResult(this);
    }
}
