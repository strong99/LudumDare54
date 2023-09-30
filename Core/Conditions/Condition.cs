namespace LudumDare54.Core.Conditions;

public interface Condition {
    Boolean IsMet(ResourceCard choice, Session session);
}
