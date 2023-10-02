using LudumDare54.Core.Scenes;
using LudumDare54.Graphics;
using Microsoft.Extensions.Logging;

namespace LudumDare54.Editor;
public static class MauiProgram {
    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

        builder.Services.AddScoped<SessionSettings, MauiSessionSettings>();
        builder.Services.AddSingleton<RepositoryFactory, MemoryRepositoryCreator>();
        builder.Services.AddScoped<QuitApplicationFeature, MauiQuitApplicationFeature>();
        builder.Services.AddScoped<SceneComposerFactory, SceneManagerFactory<LD54SceneComposer>>();
        builder.Services.AddScoped<SessionManager>();
        builder.Services.AddScoped<AudioPlayer>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
