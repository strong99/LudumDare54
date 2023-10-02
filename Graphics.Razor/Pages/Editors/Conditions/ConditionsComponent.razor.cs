using LudumDare54.Core.Conditions;
using Microsoft.AspNetCore.Components;

namespace LudumDare54.Graphics.Razor.Pages.Editors.Conditions;
public partial class ConditionsComponent {
    private Dictionary<String, Type> _availableConditionTypes = new() {
        ["Has key"] = typeof(HasKeyTagCondition),
        ["Is round"] = typeof(IsRoundCondition),
        ["Was round"] = typeof(WasRoundCondition)
    };


    [Parameter]
    public required List<Condition> Conditions { get; set; }

    protected string NewConditionType { get; set; } = "";

    protected void Add() {
        var conditionType = _availableConditionTypes[NewConditionType];
        var condition = Activator.CreateInstance(conditionType) as Condition ?? throw new ArgumentNullException();

        Conditions.Add(condition);

        NewConditionType = "";
    }
}
