using LudumDare54.Core;
using Microsoft.AspNetCore.Components;

namespace LudumDare54.Graphics.Pages.Editors;

public class EventCardPanelData : PanelData {
    public EventCard Card { get; init; }
    public EventCardPanelData(EventCard card) {
        Card = card;
    }
}

public partial class EventCardEditorPanel {
    [Parameter]
    public required EventCardPanelData Model { get; set; }

    [Parameter]
    public required EditorPanelManager Manager { get; set; }

    [Inject]
    public required RepositoryFactory RepositoryFactory { get; set; }

    private Repository _repository = default!;

    protected override async Task OnParametersSetAsync() {
        _repository = await RepositoryFactory.GetRepository();

        await base.OnParametersSetAsync();
    }

    private void Delete() {
        _repository.EventCards.Remove(Model.Card);
        Manager.Swap(Model, new EventCardsPanelData());
    }
}
