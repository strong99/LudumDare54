using LudumDare54.Core;
using LudumDare54.Core.Scenes;
using LudumDare54.Graphics;
using LudumDare54.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

Console.WriteLine($"Force load {typeof(Session).Name}");

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<SessionSettings, WebSessionSettings>();
builder.Services.AddSingleton<RepositoryFactory, MemoryRepositoryCreator>();
builder.Services.AddScoped<SessionManager>();
builder.Services.AddScoped<AudioPlayer>();
builder.Services.AddScoped<SceneComposerFactory, SceneManagerFactory<LD54SceneComposer>>();

builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();