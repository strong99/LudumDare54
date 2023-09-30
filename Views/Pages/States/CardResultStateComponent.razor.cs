using LudumDare54.Core;
using LudumDare54.Core.States;
using Microsoft.AspNetCore.Components;

namespace LudumDare54.Graphics.Pages.States;

public partial class CardResultStateComponent {
    [Parameter]
    public required CardResultState CardResultState { get; set; }

    [Parameter]
    public required CardResultState State { get; set; }

    [Parameter]
    public required Session Session { get; set; }
}
