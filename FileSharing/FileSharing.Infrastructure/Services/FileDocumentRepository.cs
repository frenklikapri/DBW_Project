using FileSharing.Common.Dtos.Files;
using FileSharing.Common.Dtos.FileUpload;
using FileSharing.Common.Dtos.PaginatedTable;
using FileSharing.Common.Extensions;
using FileSharing.Core.Entities;
using FileSharing.Infrastructure.Data;
using FleSharing.Core.Entities;
using FleSharing.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Infrastructure.Services
{
    public class FileDocumentRepository : IFileDocumentRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private List<FileDocument> _documentsToAdd = new();

        public FileDocumentRepository(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public void AddFile(FileUploadDto fileUploadDto)
        {
            var urlHostName = _configuration["UrlGenerationHostName"];
            var url = urlHostName.GenerateDocumentUrl();
            var toAdd = new FileDocument
            {
                Bytes = fileUploadDto.Bytes,
                Filename = fileUploadDto.Filename,
                IsBlocked = false,
                UserId = fileUploadDto.UserId,
                Size = fileUploadDto.Size,
                FileUrl = url,
                UploadedAt = DateTime.Now,
                LastTimeDownloadedAt = DateTime.Now
            };

            _dbContext.FileDocuments.Add(toAdd);
            _documentsToAdd.Add(toAdd);
        }

        public async Task<bool> DeleteFileAsync(string url)
        {
            var file = await _dbContext
                .FileDocuments
                .FirstOrDefaultAsync(f => f.FileUrl == url);

            if (file is null)
                return false;

            _dbContext
                .FileDocuments
                .Remove(file);

            var requests = await _dbContext
                .BlockRequests
                .Where(b => b.FileDocumentId == file.Id)
                .ToListAsync();

            foreach (var request in requests)
            {
                _dbContext
                    .BlockRequests
                    .Remove(request);
            }

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<FileToDownloadDto> GetFileBytesAsync(string url, string userId, string ip)
        {
            if (string.IsNullOrEmpty(userId))
            {
                var lastDownloaded = await _dbContext
                    .DownloadLogs
                    .Where(d => d.Ip == ip)
                    .OrderByDescending(d => d.DownloadedAt)
                    .FirstOrDefaultAsync();

                if (lastDownloaded is not null)
                {
                    var minutesDiff = lastDownloaded.DownloadedAt.MinutesBetween(DateTime.Now);

                    if (minutesDiff < 10)
                    {
                        return new FileToDownloadDto
                        {
                            DownloadLimitPassed = true,
                            LastDownloadedAt = lastDownloaded.DownloadedAt
                        };
                    }
                }
            }

            var document = await _dbContext
                .FileDocuments
                .FirstOrDefaultAsync(d => d.FileUrl == url);

            document.LastTimeDownloadedAt = DateTime.Now;

            if (string.IsNullOrEmpty(userId))
            {
                var toAdd = new DownloadLog
                {
                    DownloadedAt = DateTime.Now,
                    Ip = ip
                };

                _dbContext.DownloadLogs.Add(toAdd);
            }

            await _dbContext.SaveChangesAsync();

            return new FileToDownloadDto
            {
                Bytes = document?.Bytes,
                FileName = document?.Filename,
            };
        }

        public async Task<FileDocumentDto> GetFileInfoAsync(string url)
        {
            var document = await _dbContext
                .FileDocuments
                .FirstOrDefaultAsync(d => d.FileUrl == url);

            if (document is null)
                return null;

            var dto = new FileDocumentDto
            {
                FileUrl = document.FileUrl,
                Filename = document.Filename,
                IsBlocked = document.IsBlocked,
                Size = document.Size,
                UserId = document.UserId,
                Id = document.Id,
                UploadedAt = document.UploadedAt,
            };

            return dto;
        }

        public async Task<PaginatedListResult<FileDocumentDto>> GetFilesUploadedByUserAsync(string userId, string search, int page, int pageSize)
        {
            var query = _dbContext
                .FileDocuments
                .Where(d => d.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query
                    .Where(d => d.Filename.Contains(search) || d.FileUrl.Contains(search))
                    .AsQueryable();
            }

            var total = await query.CountAsync();

            query = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsQueryable();

            var documents = await query
                .Select(d => new FileDocumentDto
                {
                    UserId = d.UserId,
                    Filename = d.Filename,
                    FileUrl = d.FileUrl,
                    Id = d.Id,
                    IsBlocked = d.IsBlocked,
                    Size = d.Size,
                    UploadedAt = d.UploadedAt,
                }).ToListAsync();

            return new PaginatedListResult<FileDocumentDto>
            {
                Items = documents,
                CountAll = total
            };
        }

        public async Task<List<FileUploadResultDto>> SaveAddedFilesAsync()
        {
            var success = await _dbContext.SaveChangesAsync() > 0;

            if (!success)
                return new List<FileUploadResultDto>();

            var results = new List<FileUploadResultDto>();

            foreach (var file in _documentsToAdd)
            {
                results.Add(new FileUploadResultDto
                {
                    FileName = file.Filename,
                    FileUrl = file.FileUrl,
                    Success = success
                });
            }

            _documentsToAdd = new();

            return results;
        }
    }
}
