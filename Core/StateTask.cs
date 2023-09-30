namespace LudumDare54.Core;

public class StateTask<TState> where TState : State {
    public TState State { get; }
    public Task<TState> Task { get; }
    public StateTask(TState state, Task<TState> task) {
        State = state;
        Task = task;
    }
}
