namespace LudumDare54.Core.States;

public class CardResultState : State {
    public static StateTask<CardResultState> Create(string outcomeId, bool success) {
        var taskCompletionSource = new TaskCompletionSource<CardResultState>();
        return new StateTask<CardResultState>(
            new CardResultState(outcomeId, success, taskCompletionSource),
            taskCompletionSource.Task
        );
    }

    public String OutcomeId { get; set; }
    public Boolean Success { get; set; }

    private TaskCompletionSource<CardResultState> _taskCompletionSource = new();

    public CardResultState(string outcomeId, bool success, TaskCompletionSource<CardResultState> taskCompletionSource) {
        _taskCompletionSource = taskCompletionSource;
        OutcomeId = outcomeId;
        Success = success;
    }

    public void Finish() {
        if (_taskCompletionSource.Task.IsCompleted) {
            throw new Exception("This state has already been finished");
        }

        _taskCompletionSource.SetResult(this);
    }
}
