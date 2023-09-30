namespace LudumDare54.Core;

public class StateManager {

    public Action<State>? InputCallback { get; set; }

    public void Inform(State state) {
        InputCallback?.Invoke(state);
    }
}
