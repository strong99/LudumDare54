using System.Diagnostics.CodeAnalysis;

namespace LudumDare54.Core.Tags;

public class KeyTag : Tag {
    public required String Key { get; set; }
    public KeyTag() { }
    [SetsRequiredMembers]
    public KeyTag(String key) { Key = key; }
}
