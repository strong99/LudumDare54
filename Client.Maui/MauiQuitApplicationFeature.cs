using LudumDare54.Graphics.Razor;

namespace LudumDare54.Client.Maui;

public class MauiQuitApplicationFeature : QuitApplicationFeature {
    public void TryQuit() {
        Application.Current.Quit();
    }
}