using LudumDare54.Core;
using Microsoft.AspNetCore.Components;

namespace LudumDare54.Graphics.Razor.Pages.Editors;

public class EventCardsPanelData : PanelData {

}

public partial class EventCardsEditorPanel {
    [Parameter]
    public EventCardsPanelData Model { get; set; }

    [Parameter]
    public EditorPanelManager Manager { get; set; }

    [Inject]
    public RepositoryFactory RepositoryFactory { get; set; }
    private Repository _repository = default!;

    protected override async Task OnParametersSetAsync() {
        _repository = await RepositoryFactory.GetRepository();

        await base.OnParametersSetAsync();
    }

    private String _searchInput = "";

    private IEnumerable<EventCard> EventCards { get => _repository.EventCards; }
}
