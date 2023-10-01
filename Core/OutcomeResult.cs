namespace LudumDare54.Core;

public class OutcomeResult {
    public String Id { get; }
    public Boolean Success { get; }
    public Boolean End { get; }
    public OutcomeResult(String id, Boolean success, Boolean end) {
        Id = id;
        Success = success;
        End = end;
    }
}
