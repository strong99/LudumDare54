using LudumDare54.Core.States;
using Microsoft.AspNetCore.Components;

namespace LudumDare54.Graphics.Pages.States;

public partial class CardSelectionStateComponent {
    [Parameter]
    public required CardSelectionState CardSelectionState { get; set; }
}
