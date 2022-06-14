using FileSharing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FileSharing.API.Extensions
{
    public static class DbExtensions
    {
        public static void Migrate(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dataContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dataContext.Database.Migrate();
            }
        }
    }
}
