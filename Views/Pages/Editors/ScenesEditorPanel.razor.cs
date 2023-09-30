using LudumDare54.Core;
using LudumDare54.Core.Scenes;
using Microsoft.AspNetCore.Components;

namespace LudumDare54.Graphics.Pages.Editors;

public class ScenesPanelData : PanelData {

}

public partial class ScenesEditorPanel {
    [Parameter]
    public required ScenesPanelData Model { get; set; }

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

    private IEnumerable<Scene> Scenes { get => _repository.Scenes; }
}
