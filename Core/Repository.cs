using LudumDare54.Core.Scenes;

namespace LudumDare54.Core;

public class Repository {
    public List<ResourceCard> ResourceCards { get; set; } = new();
    public List<EventCard> EventCards { get; set; } = new();
    public List<Image> Images { get; set; } = new();
    public List<Scene> Scenes { get; set; } = new();
}