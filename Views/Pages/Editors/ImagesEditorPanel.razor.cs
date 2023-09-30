using LudumDare54.Core;
using LudumDare54.Core.Scenes;
using Microsoft.AspNetCore.Components;

namespace LudumDare54.Graphics.Pages.Editors;

public class ImagesPanelData : PanelData {

}

public partial class ImagesEditorPanel {
    [Parameter]
    public ImagesPanelData Model { get; set; }

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

    private IEnumerable<Image> Images { get => _repository.Images; }

    private async Task LinkFile() {
        if (RepositoryFactory is WriteableRepositoryFactory writeableRepositoryFactory) {
            await writeableRepositoryFactory.AddImage();
        }
    }
}
