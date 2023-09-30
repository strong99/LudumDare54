namespace LudumDare54.Core.Conditions;

public class IsRoundCondition : Condition {
    public Int32 Round { get; set; }
    public Boolean Not { get; set; } = false;
    public Boolean IsMet(ResourceCard choice, Session session) {
        return session.Round == Round;
    }
}