namespace LudumDare54.Core;

public class NextEventCard {
    public String EventCardId { get; set; }
    public Single Weight { get; set; }
    public NextEventCard(String eventCardId, Single weight) {
        EventCardId = eventCardId;
        Weight = weight;
    }
}
