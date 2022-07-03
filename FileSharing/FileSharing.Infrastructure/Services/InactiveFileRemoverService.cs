using FileSharing.Common.Extensions;
using FileSharing.Infrastructure.Data;
using FleSharing.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Infrastructure.Services
{
    public class InactiveFileRemoverService : IInactiveFileRemoverService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public InactiveFileRemoverService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<bool> DeleteInactiveFilesAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var inactiveFiles = (await dbContext
                .FileDocuments
                .ToListAsync())
                .Where(d => d.LastTimeDownloadedAt.DaysBetween(DateTime.Now) >= 14)
                //.Where(d => SecondsBetween(d.LastTimeDownloadedAt, DateTime.Now) >= 30)
                .ToList();

            foreach (var file in inactiveFiles)
            {
                dbContext.FileDocuments.Remove(file);
            }

            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
