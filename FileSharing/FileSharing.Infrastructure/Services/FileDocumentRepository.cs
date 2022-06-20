using FileSharing.Common.Dtos.Files;
using FileSharing.Common.Dtos.FileUpload;
using FileSharing.Common.Extensions;
using FileSharing.Core.Entities;
using FileSharing.Infrastructure.Data;
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
                UploadedAt = DateTime.UtcNow
            };

            _dbContext.FileDocuments.Add(toAdd);
            _documentsToAdd.Add(toAdd);
        }

        public async Task<FileToDownloadDto> GetFileBytesAsync(string url)
        {
            var document = await _dbContext
                .FileDocuments
                .FirstOrDefaultAsync(d => d.FileUrl == url);

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

        public async Task<List<FileDocumentDto>> GetFilesUploadedByUserAsync(string userId)
        {
            var documents = await _dbContext
                .FileDocuments
                .Where(d => d.UserId == userId)
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

            return documents;
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
