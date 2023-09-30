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

        builder.Services.AddSingleton<SessionSettings, MauiSessionSettings>();
        builder.Services.AddSingleton<RepositoryFactory, MemoryRepositoryCreator>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
