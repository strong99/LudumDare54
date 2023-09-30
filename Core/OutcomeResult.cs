namespace LudumDare54.Core;

public class OutcomeResult {
    public String Id { get; }
    public Boolean Success { get; }
    public OutcomeResult(String id, Boolean success) {
        Id = id;
        Success = success;
    }
}
