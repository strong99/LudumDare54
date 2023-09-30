using LudumDare54.Core.Tags;
using System.Diagnostics.CodeAnalysis;

namespace LudumDare54.Core;

public class ResourceCard {
    public required String Id { get; set; }
    public Boolean Consumable { get; set; } = false;
    public List<Tag> Tags { get; set; } = new();
    public ResourceCard() { }

    [SetsRequiredMembers]
    public ResourceCard(String id, Boolean consumable) {
        Id = id;
        Consumable = consumable;
    }
}
