using LudumDare54.Core.Modifiers;
using Microsoft.AspNetCore.Components;

namespace LudumDare54.Graphics.Pages.Editors.Modifiers;
public partial class ModifiersComponent {
    private Dictionary<String, Type> _availableModifierTypes = new() {
        ["Add key tag"] = typeof(AddKeyTagModifier),
        ["Add session key tag"] = typeof(AddSessionKeyTagModifier),
        ["Remove key tag"] = typeof(RemoveKeyTagModifier),
        ["Remove session key tag"] = typeof(RemoveSessionKeyTagModifier),
        ["Replace key tag"] = typeof(ReplaceKeyTagModifier),
        ["Replace session key tag"] = typeof(ReplaceSessionKeyTagModifier)
    };


    [Parameter]
    public required List<Modifier> Modifiers { get; set; }

    protected string NewModifierType { get; set; } = "";

    protected void Add() {
        if (!_availableModifierTypes.ContainsKey(NewModifierType)) {
            return;
        }

        var modifierType = _availableModifierTypes[NewModifierType];
        var modifier = Activator.CreateInstance(modifierType) as Modifier ?? throw new ArgumentNullException();

        Modifiers.Add(modifier);

        NewModifierType = "";
    }
}
