using LudumDare54.Core.Tags;
using Microsoft.AspNetCore.Components;

namespace LudumDare54.Graphics.Pages.Editors.Tags;

public partial class TagsComponent {
    [Parameter]
    public List<Tag> Tags { get; set; }

    protected string NewTagKey { get; set; }
    protected string NewTagType { get; set; } = "Tag";

    protected Dictionary<string, Type> _availableTagTypes = new() {
        ["Tag"] = typeof(KeyTag)
    };

    protected void AddTag() {
        if (string.IsNullOrWhiteSpace(NewTagKey)
         || Tags.Any(p => p.Key == NewTagKey)
        ) {
            return;
        }

        var tagType = _availableTagTypes[NewTagType];
        var tag = Activator.CreateInstance(tagType, NewTagKey) as Tag ?? throw new ArgumentNullException();

        Tags.Add(tag);

        NewTagKey = "";
        NewTagType = "Tag";
    }
}
