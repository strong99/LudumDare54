using LudumDare54.Graphics;

namespace LudumDare54.Editor;

public class MauiQuitApplicationFeature : QuitApplicationFeature {
    public void TryQuit() {
        Application.Current.Quit();
    }
}