using FileSharing.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace FileSharing.API.Extensions
{
    public static class SeedDataExtension
    {
        public static void SeedData(this UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
            }

            if (userManager.FindByEmailAsync("admin@gmail.com").Result == null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = "admin",
                    Email = "admin@gmail.com"
                };

                IdentityResult result = userManager.CreateAsync(user, "Admin123$").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }
    }
}
