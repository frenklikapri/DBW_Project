using System.Security.Claims;

namespace FileSharing.API.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetUserId(this HttpContext context)
        {
            try
            {
                var identity = context.User.Identity as ClaimsIdentity;
                var userId = identity.Claims
                    .First(c => c.Type == ClaimTypes.Sid);
                return userId.Value;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
