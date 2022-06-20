using FileSharing.Common.Dtos.Files;
using FileSharing.Common.Dtos.FileUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleSharing.Core.Interfaces
{
    public interface IFileDocumentRepository
    {
        void AddFile(FileUploadDto fileUploadDto);
        Task<List<FileUploadResultDto>> SaveAddedFilesAsync();
        Task<FileDocumentDto> GetFileInfoAsync(string url);
        Task<List<FileDocumentDto>> GetFilesUploadedByUserAsync(string userId);
        Task<FileToDownloadDto> GetFileBytesAsync(string url);
    }
}
