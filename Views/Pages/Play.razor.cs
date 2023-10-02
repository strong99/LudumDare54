using LudumDare54.Core;
using LudumDare54.Core.Scenes;
using LudumDare54.Core.States;
using LudumDare54.Core.Tags;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace LudumDare54.Graphics.Pages;

public struct ImageToRender {
    public Single PositionX { get; set; }
    public Single PositionY { get; set; }
    public Single OriginX { get; set; }
    public Single OriginY { get; set; }
    public Single Scale { get; set; }
    public String File { get; set; }
    public String Order { get; set; }
    public Branch Branch { get; set; }
}

[DebuggerDisplay("{FullPath}")]
public class Branch {
    public List<Tag> ExpectedTags { get; }
    public List<Tag> SolvedTags { get; } = new();
    public Scene Scene { get; }

    public Dictionary<Anchor, Image> Images { get; }

    public String FullPath { get; private set; }
    public String AnchorPath { get; private set; }

    public Branch(Scene scene, IEnumerable<Tag> tags) {
        Scene = scene;
        ExpectedTags = tags.ToList();
        Images = new Dictionary<Anchor, Image>();
    }
    private Branch(Scene scene, IEnumerable<Tag> tags, IReadOnlyDictionary<Anchor, Image> images) {
        Scene = scene;
        ExpectedTags = tags.ToList();
        Images = new Dictionary<Anchor, Image>(images);
    }
    public Branch Clone() {
        var b = new Branch(Scene, ExpectedTags, Images);
        return b;
    }
    public Branch Fork(Anchor anchor, Image image) {
        var matchedTags = ExpectedTags.Where(t => image.Tags.Any(x => x.Key == t.Key));
        var b = new Branch(Scene, ExpectedTags.Except(matchedTags), Images);
        b.SolvedTags.AddRange(matchedTags);
        b.Images.Add(anchor, image);
        b.AnchorPath = AnchorPath + "/" + (Images.Any() ? Images.Last().Value.Anchors.IndexOf(anchor) : Scene.Anchors.IndexOf(anchor));
        b.FullPath = FullPath + "/" + (Images.Any() ? Images.Last().Value.Anchors.IndexOf(anchor) : Scene.Anchors.IndexOf(anchor)) + ":" + image.File;
        return b;
    }
    public Branch Add(Anchor anchor, Image image) {
        var matchedTags = ExpectedTags.Where(t => image.Tags.Any(x => x.Key == t.Key));
        ExpectedTags.RemoveAll(t => matchedTags.Contains(t));
        SolvedTags.AddRange(matchedTags);
        Images.Add(anchor, image);
        return this;
    }
}

public interface SceneManager {

    IEnumerable<ImageToRender>? SceneElements { get; }

    IEnumerable<ImageToRender> GenerateNewScene(IEnumerable<Tag> tags);
    IEnumerable<ImageToRender> GenerateAltScene(IEnumerable<Tag> tags);
}

public partial class Play : SceneManager {
    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    [Inject]
    public required RepositoryFactory RepositoryFactory { get; set; }

    [Inject]
    public required SessionManager SessionManager { get; set; }

    public IEnumerable<ImageToRender>? SceneElements { get; private set; } = null;

    private Repository _repository = default!;
    private Session _session = default!;
    private StateManager _stateManager = default!;

    [Inject]
    public required AudioPlayer AudioPlayer { get; set; }

    protected override void OnInitialized() {
        AudioPlayer.Load("_content/LudumDare54.Graphics/audio/cardButtonHover.ogg");
        AudioPlayer.Load("_content/LudumDare54.Graphics/audio/cardButtonSelect.ogg");
        PlayMusic();

        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync() {
        _repository = await RepositoryFactory.GetRepository();

        _session = SessionManager.GetOrCreate();
        _stateManager = new() { InputCallback = StateChanged };

        _ = _session.Play(_repository, _stateManager).ContinueWith(t => {
            _state = new ManualState("credits", "Created for Ludum Dare 54 by Strong99 in 48 hours from Scratch, 2023/09/30 - 2023/10/01", () => {
                NavigationManager.NavigateTo("");
                StateHasChanged();
            });
            InvokeAsync(() => StateHasChanged());
        });

        await base.OnParametersSetAsync();
    }

    private State? _state;
    private Random _random = new();

    private void StateChanged(State state) {
        if (state is DeckSelectionState deckSelectionState) {
            _state = new ManualState("intro01", "The frontlines are hard. An ambush criples the defensive lines. You need to warn the city, now!.", () => {
                _state = state;
                StateHasChanged();
            });
        }
        else {
            _state = state;
        }
        StateHasChanged();
    }

    public void PlayMusic() {
        AudioPlayer.PlayMusic("_content/LudumDare54.Graphics/audio/background.ogg");
    }

    public void OnHover() {
        AudioPlayer.Play("_content/LudumDare54.Graphics/audio/menuButtonHover.ogg");
    }

    public void OnActivate() {
        AudioPlayer.Play("_content/LudumDare54.Graphics/audio/menuButtonSelect.ogg");
    }

    public IEnumerable<ImageToRender> GenerateNewScene(IEnumerable<Tag> tags) {
        return GenerateScene(tags, true);
    }

    public IEnumerable<ImageToRender> GenerateAltScene(IEnumerable<Tag> tags) {
        return GenerateScene(tags, false);
    }

    public IEnumerable<ImageToRender> GenerateScene(IEnumerable<Tag> tags, Boolean newScene = true) {
        var renderList = new List<ImageToRender>();

        var final = default(List<Branch>);
        var scenes = _repository.Scenes.OrderBy(s => Guid.NewGuid()).ToList();
        foreach (var scene in scenes) {
            var requiredTags = tags.ToList();
            var branches = GetBranches(new Branch(scene, requiredTags), scene.Anchors);
            var tagsSolvedBranches = branches.Where(b => b.SolvedTags.Any()).ToList();
            var randomizedBranches = tagsSolvedBranches.OrderBy(b => Guid.NewGuid()).ToList();

            if (!newScene) {
                randomizedBranches.AddRange(SceneElements.Select(e => e.Branch).Where(p => !p.SolvedTags.Any()));
            }

            while (randomizedBranches.Any()) {
                var randomizedBranch = randomizedBranches.First();
                var finalSet = new List<Branch>() { randomizedBranch };
                var toRequiredTags = requiredTags.Where(t => !randomizedBranch.SolvedTags.Any(x => t.Key == x.Key)).ToList();
                randomizedBranches.Remove(randomizedBranch);

                foreach (var nextBranch in randomizedBranches) {
                    if (!Conflict(nextBranch, randomizedBranch)) {
                        var matchedTags = nextBranch.SolvedTags.Where(t => toRequiredTags.Any(x => t.Key == x.Key)).ToList();
                        finalSet.Add(nextBranch);

                        foreach (var m in matchedTags) {
                            toRequiredTags.Remove(m);
                        }
                    }
                }

                if (!toRequiredTags.Any()) {
                    final = finalSet;
                    break;
                }
            }
            if (final is not null) {
                var anchors = new List<String>();
                var paths = new List<String>();
                foreach (var branch in final) {
                    var root = scene.Anchors;
                    var offsetX = 0.0f;
                    var offsetY = 0.0f;
                    var anchorPath = "";
                    var path = "";
                    foreach (var piece in branch.Images) {
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
                            Branch = branch
                        });
                    }
                }

                var i = 0;
                //var list = new List<ImageToRender>();
                foreach (var anchor in scene.Anchors) {
                    renderList.AddRange(RenderAnchor(anchor, new Branch(scene, new Tag[0]), anchors, paths, "/" + i, "/" + i));
                    ++i;
                }

                break;
            }
        }

        return SceneElements = renderList.OrderBy(i => i.Order).ToList();
    }





    private List<ImageToRender> RenderAnchor(Anchor anchor, Branch previous, List<String> anchors, List<String> paths, String anchorPath = "", String path = "", Single offsetX = 0, Single offsetY = 0) {
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
                visual = weightedItems.Last(p => idx < p.Key).Value;
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
        var branch = previous.Fork(anchor, visual);
        if (shouldRenderVisual) {
            list.Add(new() {
                File = visual.File,
                Order = anchorPath,
                OriginX = visual.OriginX,
                OriginY = visual.OriginY,
                PositionX = anchor.PositionX,
                PositionY = anchor.PositionY,
                Scale = anchor.Scale,
                Branch = branch
            });
        }

        foreach (var subAnchor in visual.Anchors) {
            var subAnchorPath = anchorPath + "/" + i;
            var subPath = path + "/" + i;
            list.AddRange(RenderAnchor(subAnchor, branch, anchors, paths, subAnchorPath, subPath));
            i++;
        }

        paths.Add(path);
        anchors.Add(anchorPath);
        return list;
    }

    private Boolean Conflict(Branch a, Branch b) {
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

    private List<Branch> GetBranches(Branch branch, IEnumerable<Anchor> anchors) {
        var newBranches = new List<Branch>();
        var unchanged = branch.Clone();
        foreach (var anchor in anchors) {
            var visuals = GetVisuals(anchor).ToList();
            foreach (var visual in visuals) {
                var newBranch = branch.Fork(anchor, visual);
                if (newBranch.ExpectedTags.Count < unchanged.ExpectedTags.Count) {
                    newBranches.Add(newBranch);
                }
                if (visual.Anchors.Any()) {
                    var foundBranches = GetBranches(newBranch, visual.Anchors);
                    newBranches.AddRange(foundBranches);
                }
            }
        }
        return newBranches;
    }

}

public class ManualState : State {
    public String Id { get; }
    public String Text { get; }
    private Action _action;

    public ManualState(String id, String text, Action action) {
        Id = id;
        Text = text;
        _action = action;
    }

    public void Finish() {
        _action.Invoke();
    }
}