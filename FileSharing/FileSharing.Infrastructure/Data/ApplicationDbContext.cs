using FileSharing.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<FileDocument> FileDocuments { get; set; }
        public DbSet<DownloadLog> DownloadLogs { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
