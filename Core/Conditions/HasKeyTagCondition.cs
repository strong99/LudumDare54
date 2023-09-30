namespace LudumDare54.Core.Conditions;

public class HasKeyTagCondition : Condition {
    public String Key { get; set; }
    public Boolean Not { get; set; } = false;
    public Boolean IsMet(ResourceCard choice, Session session) {
        return session.Tags.Any(p => p.Key.Equals(Key)) || choice.Tags.Any(p => p.Key.Equals(Key));
    }
}