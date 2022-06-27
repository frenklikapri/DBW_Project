using Blazored.LocalStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FileSharing.App.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetUserId(this string authToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(authToken.Replace("\"", ""));
            var userId = jwt.Claims.First(claim => claim.Type == ClaimTypes.Sid)?.Value;
            return userId;
        }

        public static async Task<string> GetUserId(this ILocalStorageService localStorage)
        {
            if (!(await localStorage.ContainKeyAsync("authToken")))
                return string.Empty;

            var authToken = await localStorage.GetItemAsStringAsync("authToken");
            authToken = authToken.Replace("\"", "");
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(authToken.Replace("\"", ""));
            var userId = jwt.Claims.First(claim => claim.Type == ClaimTypes.Sid)?.Value;
            return userId;
        }

        public static async Task<bool> ContainsIdentityRole(this ILocalStorageService localStorage, string role)
        {
            if (!(await localStorage.ContainKeyAsync("authToken")))
                return false;

            var authToken = await localStorage.GetItemAsStringAsync("authToken");
            authToken = authToken.Replace("\"", "");
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(authToken.Replace("\"", ""));
            var roles = jwt.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
            var hasRole = roles.Any(r => r.Value == role);
            return hasRole;
        }
    }
}
