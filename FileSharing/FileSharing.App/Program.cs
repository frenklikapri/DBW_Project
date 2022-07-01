using FileSharing.App;
using FileSharing.App.Extensions;
using Fluxor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["APIBaseUrl"]) });

builder.AddCommonServices();

builder.Services.AddFluxor(o => o
  .ScanAssemblies(typeof(Program).Assembly));

await builder.Build().RunAsync();
