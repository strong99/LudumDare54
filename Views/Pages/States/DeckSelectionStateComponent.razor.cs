using LudumDare54.Core.States;
using Microsoft.AspNetCore.Components;

namespace LudumDare54.Graphics.Pages.States;

public partial class DeckSelectionStateComponent {
    [Parameter]
    public required DeckSelectionState DeckSelectionState { get; set; }
}
