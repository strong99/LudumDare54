using LudumDare54.Core;
using Microsoft.AspNetCore.Components;

namespace LudumDare54.Graphics.Pages;

public partial class Play {

    [Inject]
    public required RepositoryFactory RepositoryFactory { get; set; }
    
    [Inject]
    public required SessionManager SessionManager { get; set; }

    private Repository _repository = default!;
    private Session _session = default!;
    private StateManager _stateManager = default!;

    protected override async Task OnParametersSetAsync() {
        _repository = await RepositoryFactory.GetRepository();

        _session = SessionManager.GetOrCreate();
        _stateManager = new() { InputCallback = StateChanged };

        _ = _session.Play(_repository, _stateManager);

        await base.OnParametersSetAsync();
    }

    private State? _state;

    private void StateChanged(State state) {
        _state = state;
        StateHasChanged();
    }
}
