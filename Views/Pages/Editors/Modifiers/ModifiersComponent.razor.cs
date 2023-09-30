using LudumDare54.Core.Modifiers;
using Microsoft.AspNetCore.Components;

namespace LudumDare54.Graphics.Pages.Editors.Modifiers;
public partial class ModifiersComponent {
    private Dictionary<String, Type> _availableModifierTypes = new() {
        ["Add key tag"] = typeof(AddKeyTagModifier),
        ["Remove key tag"] = typeof(RemoveKeyTagModifier)
    };


    [Parameter]
    public required List<Modifier> Modifiers { get; set; }

    protected string NewModifierType { get; set; } = "Add key tag";

    protected void Add() {
        var modifierType = _availableModifierTypes[NewModifierType];
        var modifier = Activator.CreateInstance(modifierType) as Modifier ?? throw new ArgumentNullException();

        Modifiers.Add(modifier);

        NewModifierType = _availableModifierTypes.Keys.First();
    }
}
