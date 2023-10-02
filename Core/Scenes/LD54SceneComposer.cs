using LudumDare54.Core.Tags;
using System.Diagnostics;
using static System.Formats.Asn1.AsnWriter;

namespace LudumDare54.Core.Scenes;

public struct ImageToRender {
    public Single PositionX { get; set; }
    public Single PositionY { get; set; }
    public Single OriginX { get; set; }
    public Single OriginY { get; set; }
    public Single Scale { get; set; }
    public String File { get; set; }
    public String Order { get; set; }
    public ScenePath Path { get; set; }
}

[DebuggerDisplay("{FullPath}")]
public class ScenePath {
    public List<Tag> ExpectedTags { get; }
    public List<Tag> SolvedTags { get; } = new();
    public Scene Scene { get; }

    public Dictionary<Anchor, Image> Images { get; }

    public String FullPath { get; private set; }
    public String AnchorPath { get; private set; }

    public ScenePath(Scene scene, IEnumerable<Tag> tags) {
        Scene = scene;
        ExpectedTags = tags.ToList();
        Images = new Dictionary<Anchor, Image>();
    }
    private ScenePath(Scene scene, IEnumerable<Tag> tags, IReadOnlyDictionary<Anchor, Image> images) {
        Scene = scene;
        ExpectedTags = tags.ToList();
        Images = new Dictionary<Anchor, Image>(images);
    }
    public ScenePath Clone() {
        var b = new ScenePath(Scene, ExpectedTags, Images);
        return b;
    }
    public ScenePath Fork(Anchor anchor, Image image) {
        var matchedTags = ExpectedTags.Where(t => image.Tags.Any(x => x.Key == t.Key));
        var b = new ScenePath(Scene, ExpectedTags.Except(matchedTags), Images);
        b.SolvedTags.AddRange(matchedTags);
        b.Images.Add(anchor, image);
        b.AnchorPath = AnchorPath + "/" + (Images.Any() ? Images.Last().Value.Anchors.IndexOf(anchor) : Scene.Anchors.IndexOf(anchor));
        b.FullPath = FullPath + "/" + (Images.Any() ? Images.Last().Value.Anchors.IndexOf(anchor) : Scene.Anchors.IndexOf(anchor)) + ":" + image.File;
        return b;
    }
    public ScenePath Add(Anchor anchor, Image image) {
        var matchedTags = ExpectedTags.Where(t => image.Tags.Any(x => x.Key == t.Key));
        ExpectedTags.RemoveAll(t => matchedTags.Contains(t));
        SolvedTags.AddRange(matchedTags);
        Images.Add(anchor, image);
        return this;
    }
}

public class LD54SceneComposer : SceneComposer {
    private readonly Repository _repository;
    private readonly Random _random = new();

    public IEnumerable<ImageToRender>? SceneElements { get; private set; } = null;

    public LD54SceneComposer(Repository repository) {
        _repository = repository;
    }

    public IEnumerable<ImageToRender> GenerateNewScene(IEnumerable<Tag> tags) 
        => GenerateScene(tags, true);

    public IEnumerable<ImageToRender> GenerateAltScene(IEnumerable<Tag> tags) 
        => GenerateScene(tags, false);

    public IEnumerable<ImageToRender> GenerateScene(IEnumerable<Tag> tags, Boolean newScene = true) {
        var renderList = new List<ImageToRender>();
        var renderScene = default(Scene);
        var anchors = new List<String>();
        var paths = new List<String>();

        var final = default(List<ScenePath>);
        var scenes = _repository.Scenes.OrderBy(s => Guid.NewGuid()).ToList();
        foreach (var scene in scenes) {
            var requiredTags = tags.ToList();
            var ScenePathes = GetScenePathes(new ScenePath(scene, requiredTags), scene.Anchors);
            var tagsSolvedScenePathes = ScenePathes.Where(b => b.SolvedTags.Any()).ToList();
            var randomizedScenePathes = tagsSolvedScenePathes.OrderBy(b => Guid.NewGuid()).ToList();

            if (!newScene) {
                randomizedScenePathes.AddRange(SceneElements.Select(e => e.Path).Where(p => !p.SolvedTags.Any()));
            }

            while (randomizedScenePathes.Any()) {
                var randomizedScenePath = randomizedScenePathes.First();
                var finalSet = new List<ScenePath>() { randomizedScenePath };
                var toRequiredTags = requiredTags.Where(t => !randomizedScenePath.SolvedTags.Any(x => t.Key == x.Key)).ToList();
                randomizedScenePathes.Remove(randomizedScenePath);

                foreach (var nextScenePath in randomizedScenePathes) {
                    if (!Conflict(nextScenePath, randomizedScenePath)) {
                        var matchedTags = nextScenePath.SolvedTags.Where(t => toRequiredTags.Any(x => t.Key == x.Key)).ToList();
                        finalSet.Add(nextScenePath);

                        foreach (var m in matchedTags) {
                            toRequiredTags.Remove(m);
                        }
                    }
                }

                if (!toRequiredTags.Any()) {
                    final = finalSet;
                    renderScene = scene;
                    break;
                }
            }
            if (final is not null) {
                foreach (var ScenePath in final) {
                    var root = scene.Anchors;
                    var offsetX = 0.0f;
                    var offsetY = 0.0f;
                    var anchorPath = "";
                    var path = "";
                    foreach (var piece in ScenePath.Images) {
                        var anchor = piece.Key;
                        var image = piece.Value;
                        anchorPath += "/" + root.IndexOf(anchor);
                        path += "/" + root.IndexOf(anchor) + ":" + image.File;
                        //offsetX += anchor.PositionX;
                        //offsetY += anchor.PositionY;
                        offsetX = image.OriginX;
                        offsetY = image.OriginY;
                        root = image.Anchors;
                        if (paths.Contains(path)) {
                            continue;
                        }
                        anchors.Add(anchorPath);
                        paths.Add(path);
                        renderList.Add(new() {
                            File = image.File,
                            Order = anchorPath,
                            PositionX = anchor.PositionX,
                            PositionY = anchor.PositionY,
                            OriginX = image.OriginX,
                            OriginY = image.OriginY,
                            Scale = anchor.Scale,
                            Path = ScenePath
                        });
                    }
                }

                break;
            }
        }

        if (renderScene is null) {
            renderScene = scenes[_random.Next(scenes.Count)];
        }

        var i = 0;
        foreach (var anchor in renderScene.Anchors) {
            renderList.AddRange(RenderAnchor(anchor, new ScenePath(renderScene, Array.Empty<Tag>()), anchors, paths, "/" + i, "/" + i));
            ++i;
        }

        return SceneElements = renderList.OrderBy(i => i.Order).ToList();
    }

    private List<ImageToRender> RenderAnchor(Anchor anchor, ScenePath previous, List<String> anchors, List<String> paths, String anchorPath = "", String path = "", Single offsetX = 0, Single offsetY = 0) {
        var list = new List<ImageToRender>();
        var shouldRenderVisual = !anchors.Contains(anchorPath);
        Image? visual = null;
        if (anchor is StaticSelectingAnchor staticSelectingAnchor) {
            if (shouldRenderVisual) {
                var query = _repository.Images.Where(p => p.Weight > 0 && staticSelectingAnchor.Tags.All(t => p.Tags.Any(x => x.Key.Equals(t.Key, StringComparison.OrdinalIgnoreCase)))).ToList();
                if (!query.Any()) {
                    return list;
                }
                var maxScore = 0.0f;
                var weightedItems = new Dictionary<Single, Image>();
                foreach (var item in query) {
                    maxScore += item.Weight;
                    weightedItems.Add(maxScore, item);
                }
                var idx = _random.NextDouble() * maxScore;
                visual = weightedItems.First(p => idx < p.Key).Value;
            }
            else {
                visual = _repository.Images.FirstOrDefault(i => paths.Contains(path + ":" + i.File));
            }
        }
        if (visual is null || _random.Next(100) > anchor.Chance) {
            return list;
        }

        var i = 0;

        path += ":" + visual.File;
        var ScenePath = previous.Fork(anchor, visual);
        if (shouldRenderVisual) {
            list.Add(new() {
                File = visual.File,
                Order = anchorPath,
                OriginX = visual.OriginX,
                OriginY = visual.OriginY,
                PositionX = anchor.PositionX,
                PositionY = anchor.PositionY,
                Scale = anchor.Scale,
                Path = ScenePath
            });
        }

        foreach (var subAnchor in visual.Anchors) {
            var subAnchorPath = anchorPath + "/" + i;
            var subPath = path + "/" + i;
            list.AddRange(RenderAnchor(subAnchor, ScenePath, anchors, paths, subAnchorPath, subPath));
            i++;
        }

        paths.Add(path);
        anchors.Add(anchorPath);
        return list;
    }

    private Boolean Conflict(ScenePath a, ScenePath b) {
        var aIt = a.Images.GetEnumerator();
        var bIt = b.Images.GetEnumerator();
        while (aIt.MoveNext()) {
            bIt.MoveNext();

            if (aIt.Current.Key != bIt.Current.Key) {
                break;
            }
            else if (aIt.Current.Value.File != bIt.Current.Value.File) {
                return true;
            }
        }
        return false;
    }

    private IEnumerable<Image> GetVisuals(Anchor anchor) {
        if (anchor is StaticSelectingAnchor staticSelectingAnchor) {
            return _repository.Images.Where(p => staticSelectingAnchor.Tags.All(t => p.Tags.Any(x => x.Key.Equals(t.Key, StringComparison.OrdinalIgnoreCase))));
        }
        else if (anchor is StaticAnchor staticAnchor) {
            return new[] {
                _repository.Images.First(i => i.File == staticAnchor.ImageFile)
    };
        }
        else throw new NotImplementedException();
    }

    private List<ScenePath> GetScenePathes(ScenePath ScenePath, IEnumerable<Anchor> anchors) {
        var newScenePathes = new List<ScenePath>();
        var unchanged = ScenePath.Clone();
        foreach (var anchor in anchors) {
            var visuals = GetVisuals(anchor).ToList();
            foreach (var visual in visuals) {
                var newScenePath = ScenePath.Fork(anchor, visual);
                if (newScenePath.ExpectedTags.Count < unchanged.ExpectedTags.Count) {
                    newScenePathes.Add(newScenePath);
                }
                if (visual.Anchors.Any()) {
                    var foundScenePathes = GetScenePathes(newScenePath, visual.Anchors);
                    newScenePathes.AddRange(foundScenePathes);
                }
            }
        }
        return newScenePathes;
    }

}
