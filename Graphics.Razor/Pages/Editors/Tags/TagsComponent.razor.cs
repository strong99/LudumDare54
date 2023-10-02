using LudumDare54.Core.Tags;
using Microsoft.AspNetCore.Components;

namespace LudumDare54.Graphics.Razor.Pages.Editors.Tags;

public partial class TagsComponent {
    [Parameter]
    public List<Tag> Tags { get; set; }

    protected String NewTagKey { get; set; } = "";

    protected String NewTagType { get; set; } = "";

    protected Dictionary<string, Type> _availableTagTypes = new() {
        ["Tag"] = typeof(KeyTag)
    };

    protected void AddTag() {
        if (string.IsNullOrWhiteSpace(NewTagKey)
         || Tags.Any(p => p.Key == NewTagKey)
        ) {
            return;
        }

        if (!_availableTagTypes.ContainsKey(NewTagType)) {
            return;
        }

        var tagType = _availableTagTypes[NewTagType];
        var tag = Activator.CreateInstance(tagType, NewTagKey) as Tag ?? throw new ArgumentNullException();

        Tags.Add(tag);

        NewTagKey = "";
        NewTagType = "";
    }
}
