using LudumDare54.Core.Scenes;
using LudumDare54.Core;
using LudumDare54.Core.States;
using LudumDare54.Core.Tags;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LudumDare54.Graphics.Pages.States;

public partial class CardSelectionStateComponent {
    [Parameter]
    public required CardSelectionState CardSelectionState { get; set; }

    [Inject]
    public required AudioPlayer AudioPlayer { get; set; }
    [Inject]
    public required IJSRuntime JSRuntime { get; set; }

    private Guid _innerGuid;
    private Random _random = new();
    private Int32 _randomIdx;

    protected override void OnInitialized() {
        _innerGuid = Guid.NewGuid();
        _randomIdx = _random.Next(0, 1000000);
        base.OnInitialized();
    }

    protected override void OnAfterRender(Boolean firstRender) {
        JSRuntime.InvokeAsync<String>("resizePreview", _innerGuid);

        base.OnAfterRender(firstRender);
    }


    [Parameter]
    public required CardSelectionState State { get; set; }

    [Parameter]
    public required Session Session { get; set; }

    [Inject]
    public required RepositoryFactory RepositoryFactory { get; set; }

    private Repository _repository = default!;

    protected void Toggle(String eventCardId) {
        Session.Deck ??= new();
        if (Session.Deck.Contains(eventCardId)) {
            Session.Deck.Remove(eventCardId);
        }
        else {
            Session.Deck.Add(eventCardId);
        }
    }

    protected override async Task OnParametersSetAsync() {
        _repository = await RepositoryFactory.GetRepository();

        await base.OnParametersSetAsync();
    }

    struct ImageToRender {
        public Single PositionX { get; set; }
        public Single PositionY { get; set; }
        public Single OriginX { get; set; }
        public Single OriginY { get; set; }
        public Single Scale { get; set; }
        public String File { get; set; }
        public String Order { get; set; }
    }

    private List<ImageToRender> RenderScene() {
        var renderList = new List<ImageToRender>();
        var tags = new List<Tag>(Session.Tags);
        tags.AddRange(State.EventCard.Tags.Where(t => !tags.Any(x => x.Key == t.Key)));

        var final = default(List<Branch>);
        var scenes = _repository.Scenes.OrderBy(s => Guid.NewGuid()).ToList();
        foreach (var scene in scenes) {
            var requiredTags = tags.ToList();
            var branches = GetBranches(new Branch(tags), scene.Anchors);
            var tagsSolvedBranches = branches.Where(b => b.SolvedTags.Any()).ToList();
            var randomizedBranches = tagsSolvedBranches.OrderBy(b => Guid.NewGuid()).ToList();

            while (randomizedBranches.Any()) {
                var randomizedBranch = randomizedBranches.First();
                var finalSet = new List<Branch>() { randomizedBranch };
                var toRequiredTags = requiredTags.Where(t => !randomizedBranch.SolvedTags.Any(x => t.Key == x.Key)).ToList();
                randomizedBranches.Remove(randomizedBranch);

                foreach (var nextBranch in randomizedBranches) {
                    if (!Conflict(nextBranch, randomizedBranch)) {
                        var matchedTags = nextBranch.SolvedTags.Where(t => toRequiredTags.Any(x => t.Key == x.Key)).ToList();
                        if (matchedTags.Any()) {
                            finalSet.Add(nextBranch);

                            foreach (var m in matchedTags) {
                                toRequiredTags.Remove(m);
                            }
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
                            Scale = anchor.Scale
                        });
                    }
                }

                var i = 0;
                //var list = new List<ImageToRender>();
                foreach (var anchor in scene.Anchors) {
                    renderList.AddRange(RenderAnchor(anchor, anchors, paths, "/" + i, "/" + i));
                    ++i;
                }

                break;
            }
        }
        return renderList;
    }

    private List<ImageToRender> RenderAnchor(Anchor anchor, List<String> anchors, List<String> paths, String anchorPath = "", String path = "", Single offsetX = 0, Single offsetY = 0) {
        var list = new List<ImageToRender>();
        var shouldRenderVisual = !anchors.Contains(anchorPath);
        Image? visual = null;
        if (anchor is StaticSelectingAnchor staticSelectingAnchor) {
            if (shouldRenderVisual) {
                var query = _repository.Images.Where(p => staticSelectingAnchor.Tags.All(t => p.Tags.Any(x => x.Key.Equals(t.Key, StringComparison.OrdinalIgnoreCase))));
                visual = query.Skip(_random.Next(query.Count())).FirstOrDefault();
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
        if (shouldRenderVisual) {
            list.Add(new() {
                File = visual.File,
                Order = anchorPath,
                OriginX = visual.OriginX,
                OriginY = visual.OriginY,
                PositionX = anchor.PositionX,
                PositionY = anchor.PositionY,
                Scale = anchor.Scale
            });
        }

        foreach (var subAnchor in visual.Anchors) {
            var subAnchorPath = anchorPath + "/" + i;
            var subPath = path + "/" + i;
            list.AddRange(RenderAnchor(subAnchor, anchors, paths, subAnchorPath, subPath));
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

    class Branch {
        public List<Tag> ExpectedTags { get; }
        public List<Tag> SolvedTags { get; } = new();

        public Branch(List<Tag> expectedTags) {
            ExpectedTags = expectedTags;
            Images = new();
        }

        public Dictionary<Anchor, Image> Images { get; }

        public Branch(IEnumerable<Tag> tags) {
            ExpectedTags = tags.ToList();
            Images = new Dictionary<Anchor, Image>();
        }
        private Branch(IEnumerable<Tag> tags, IReadOnlyDictionary<Anchor, Image> images) {
            ExpectedTags = tags.ToList();
            Images = new Dictionary<Anchor, Image>(images);
        }
        public Branch Clone() {
            var b = new Branch(ExpectedTags, Images);
            return b;
        }
        public Branch Fork(Anchor anchor, Image image) {
            var matchedTags = ExpectedTags.Where(t => image.Tags.Any(x => x.Key == t.Key));
            var b = new Branch(ExpectedTags.Except(matchedTags), Images);
            b.SolvedTags.AddRange(matchedTags);
            b.Images.Add(anchor, image);
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
                if (visual.Anchors.Any()) {
                    var foundBranches = GetBranches(newBranch, visual.Anchors);
                    newBranches.AddRange(foundBranches);
                }
                else if (newBranch.ExpectedTags.Count < unchanged.ExpectedTags.Count) {
                    newBranches.Add(newBranch);
                }
            }
        }
        return newBranches;
    }

    private void OnCardHover() {
        AudioPlayer.Play("_content/LudumDare54.Graphics/audio/cardButtonHover.wav");
    }

    private void OnCardSelect() {
        AudioPlayer.Play("_content/LudumDare54.Graphics/audio/cardButtonSelect.wav");
    }
}
