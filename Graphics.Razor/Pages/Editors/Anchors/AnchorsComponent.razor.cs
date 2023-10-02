using LudumDare54.Core.Scenes;
using Microsoft.AspNetCore.Components;

namespace LudumDare54.Graphics.Razor.Pages.Editors.Anchors;

public partial class AnchorsComponent {
    [Parameter]
    public List<Anchor> Anchors { get; set; }

    protected string NewAnchorType { get; set; } = "Static";

    protected Dictionary<string, Type> _availableAnchorTypes = new() {
        ["Static"] = typeof(StaticAnchor),
        ["Static selection"] = typeof(StaticSelectingAnchor)
    };

    protected void AddAnchor() {
        var anchorType = _availableAnchorTypes[NewAnchorType];
        var anchor = Activator.CreateInstance(anchorType) as Anchor ?? throw new ArgumentNullException();

        Anchors.Add(anchor);

        NewAnchorType = _availableAnchorTypes.Keys.First();
    }

    protected void Up(Anchor anchor) {
        var idx = Anchors.IndexOf(anchor);
        if (idx == 0) return;

        Anchors.RemoveAt(idx);
        Anchors.Insert(idx - 1, anchor);
    }

    protected void Down(Anchor anchor) {
        var idx = Anchors.IndexOf(anchor);
        if (idx == Anchors.Count - 1) return;

        Anchors.RemoveAt(idx);
        Anchors.Insert(idx + 1, anchor);
    }
}
