using LudumDare54.Core.Scenes;
using LudumDare54.Core.Tags;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LudumDare54.Graphics.Pages.Editors;

public class SceneViewerPanelData : PanelData {
}

public partial class SceneViewerPanel {
    [Parameter]
    public required SceneViewerPanelData Model { get; set; }

    [Parameter]
    public required EditorPanelManager Manager { get; set; }

    [Inject]
    public required RepositoryFactory RepositoryFactory { get; set; }

    [Inject]
    public required IJSRuntime JSRuntime { get; set; }

    [Inject]
    public required SceneComposerFactory SceneComposerFactory { get; set; }
    private SceneComposer _sceneComposer = default!;

    private Guid _innerGuid = Guid.NewGuid();
    private readonly List<Tag> _tags = new();

    protected override void OnInitialized() {
        _innerGuid = Guid.NewGuid();
        base.OnInitialized();
    }

    protected override void OnAfterRender(Boolean firstRender) {
        _ = JSRuntime.InvokeAsync<String>("resizePreview", _innerGuid);
        base.OnAfterRender(firstRender);
    }

    protected override async Task OnParametersSetAsync() {
        _sceneComposer ??= await SceneComposerFactory.Create();
        _sceneComposer.GenerateNewScene(_tags);
        await base.OnParametersSetAsync();
    }
}
