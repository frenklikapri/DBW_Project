using FileSharing.Common.Dtos.FileUpload;
using FileSharing.Core.Entities;
using FileSharing.Infrastructure.Data;
using FleSharing.Core.Interfaces;
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
        private List<FileDocument> _documentsToAdd = new();

        public FileDocumentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddFile(FileUploadDto fileUploadDto)
        {
            var toAdd = new FileDocument
            {
                Bytes = fileUploadDto.Bytes,
                Filename = fileUploadDto.Filename,
                IsBlocked = false,
                UserId = fileUploadDto.UserId,
                Size = fileUploadDto.Size,
                FileUrl = "TODO"
            };

            _dbContext.FileDocuments.Add(toAdd);
            _documentsToAdd.Add(toAdd);
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
