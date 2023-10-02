using LudumDare54.Core;
using Microsoft.AspNetCore.Components;

namespace LudumDare54.Graphics.Razor.Pages.Editors;

public class ResourceCardPanelData : PanelData {
    public ResourceCard Card { get; init; }
    public ResourceCardPanelData(ResourceCard card) {
        Card = card;
    }
}

public partial class ResourceCardEditorPanel {
    [Parameter]
    public ResourceCardPanelData Model { get; set; }

    [Parameter]
    public EditorPanelManager Manager { get; set; }

    [Inject]
    public RepositoryFactory RepositoryFactory { get; set; }

    private Repository _repository = default!;

    protected override async Task OnParametersSetAsync() {
        _repository = await RepositoryFactory.GetRepository();

        await base.OnParametersSetAsync();
    }

    private void Delete() {
        _repository.ResourceCards.Remove(Model.Card);
        Manager.Swap(Model, new ResourceCardsPanelData());
    }
}
