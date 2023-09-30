﻿using LudumDare54.Core;
using LudumDare54.Graphics;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace LudumDare54.Graphics.Pages;

public interface PanelData {

}

public interface EditorPanelManager {
    void Add(PanelData panel);
    void After(PanelData panel, PanelData newPanel);
    void Remove(PanelData panel);
    void Swap(PanelData from, PanelData to);
}

public partial class Edit : EditorPanelManager {
    private readonly List<PanelData> _panels = new();

    [Inject]
    public RepositoryFactory RepositoryFactory { get; set; }
    public Boolean IsWriteable { get => RepositoryFactory is WriteableRepositoryFactory; }

    private Repository _repository = default!;

    protected override async Task OnParametersSetAsync() {
        _repository = await RepositoryFactory.GetRepository();

        await base.OnParametersSetAsync();
    }

    public void Swap(PanelData from, PanelData to) {
        var idx = _panels.IndexOf(from);
        _panels[idx] = to;
        StateHasChanged();
    }
    public void Add(PanelData from) {
        _panels.Add(from);
        StateHasChanged();
    }
    public void After(PanelData panel, PanelData newPanel) {
        var idx = _panels.IndexOf(panel);
        _panels.Insert(idx + 1, newPanel);
        StateHasChanged();

    }
    public void Remove(PanelData from) {
        _panels.Remove(from);
        StateHasChanged();
    }
    public void Save() {
        if (RepositoryFactory is WriteableRepositoryFactory writeableRepositoryFactory) {
            writeableRepositoryFactory.Save();
            StateHasChanged();
        }
    }
}
