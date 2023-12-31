﻿using LudumDare54.Core;
using Microsoft.AspNetCore.Components;

namespace LudumDare54.Graphics.Pages.Editors;

public class ResourceCardsPanelData : PanelData {

}

public partial class ResourceCardsEditorPanel {
    [Parameter]
    public required ResourceCardsPanelData Model { get; set; }

    [Parameter]
    public required EditorPanelManager Manager { get; set; }

    [Inject]
    public required RepositoryFactory RepositoryFactory { get; set; }


    private Repository _repository = default!;

    protected override async Task OnParametersSetAsync() {
        _repository = await RepositoryFactory.GetRepository();

        await base.OnParametersSetAsync();
    }

    private String _searchInput = "";

    private IEnumerable<ResourceCard> ResourceCards { get => _repository.ResourceCards; }
}
