using Blazored.LocalStorage;
using FileSharing.App.Auth;
using FileSharing.App.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace FileSharing.App.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static WebAssemblyHostBuilder AddCommonServices(this WebAssemblyHostBuilder builder)
        {
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

            return builder;
        }
    }
}
