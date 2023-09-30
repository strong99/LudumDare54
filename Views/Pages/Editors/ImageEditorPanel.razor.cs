using LudumDare54.Core;
using LudumDare54.Core.Scenes;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LudumDare54.Graphics.Pages.Editors;

public class ImagePanelData : PanelData {
    public Image Image { get; init; }
    public ImagePanelData(Image image) {
        Image = image;
    }
}

public partial class ImageEditorPanel {
    [Parameter]
    public required ImagePanelData Model { get; set; }

    [Parameter]
    public required EditorPanelManager Manager { get; set; }

    [Inject]
    public required RepositoryFactory RepositoryFactory { get; set; }

    private Repository _repository = default!;

    [Inject]
    public required IJSRuntime JSRuntime { get; set; }

    private Guid _innerGuid;
    private Random _random = new();
    private Int32 _randomIdx;

    protected override void OnInitialized() {
        _innerGuid = Guid.NewGuid();
        _randomIdx = _random.Next(0, 1000000);
        base.OnInitialized();
    }

    protected override void OnAfterRender(Boolean firstRender) {
        JSRuntime.InvokeAsync<String>("resizePreview", _innerGuid);

        base.OnAfterRender(firstRender);
    }

    protected override async Task OnParametersSetAsync() {
        _repository = await RepositoryFactory.GetRepository();

        await base.OnParametersSetAsync();
    }

    private void Delete() {
        _repository.Images.Remove(Model.Image);
        Manager.Swap(Model, new ImagesPanelData());
    }
}
