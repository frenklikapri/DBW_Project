using Blazored.LocalStorage;
using FileSharing.App.Auth;
using FileSharing.App.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace FileSharing.App.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static WebApplicationBuilder AddCommonServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

            return builder;
        }
    }
}
