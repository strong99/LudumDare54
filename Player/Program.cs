using LudumDare54.Web;
using LudumDare54.Graphics;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LudumDare54.Core;

Console.WriteLine($"Force load {typeof(Session).Name}");

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<SessionSettings, WebSessionSettings>();
builder.Services.AddSingleton<RepositoryFactory, MemoryRepositoryCreator>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();