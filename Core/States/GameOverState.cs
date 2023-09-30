namespace LudumDare54.Core.States;

public class GameOverState : State {
    public static StateTask<GameOverState> Create() {
        var taskCompletionSource = new TaskCompletionSource<GameOverState>();
        return new StateTask<GameOverState>(
            new GameOverState(taskCompletionSource),
            taskCompletionSource.Task
        );
    }

    private TaskCompletionSource<GameOverState> _taskCompletionSource = new();

    public GameOverState(TaskCompletionSource<GameOverState> taskCompletionSource) {
        _taskCompletionSource = taskCompletionSource;
    }

    public void Finish() {
        if (_taskCompletionSource.Task.IsCompleted) {
            throw new Exception("This state has already been finished");
        }

        _taskCompletionSource.SetResult(this);
    }
}
