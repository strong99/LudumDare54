using LudumDare54.Core;
using LudumDare54.Core.Scenes;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LudumDare54.Graphics.Pages.Editors;

public class ScenePanelData : PanelData {
    public Scene Scene { get; init; }
    public ScenePanelData(Scene scene) {
        Scene = scene;
    }
}

public partial class SceneEditorPanel {
    [Parameter]
    public required ScenePanelData Model { get; set; }

    [Parameter]
    public required EditorPanelManager Manager { get; set; }

    [Inject]
    public required RepositoryFactory RepositoryFactory { get; set; }

    private Repository _repository = default!;

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

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

        Model.Scene.Tags ??= new();
        Model.Scene.Anchors ??= new();

        await base.OnParametersSetAsync();
    }

    private void Delete() {
        _repository.Scenes.Remove(Model.Scene);
        Manager.Swap(Model, new ScenesPanelData());
    }
}
