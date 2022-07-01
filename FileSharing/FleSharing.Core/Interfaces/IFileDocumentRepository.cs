using FileSharing.Common.Dtos.Files;
using FileSharing.Common.Dtos.FileUpload;
using FileSharing.Common.Dtos.PaginatedTable;
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
        Task<PaginatedListResult<FileDocumentDto>> GetFilesUploadedByUserAsync(string userId, string search, int page, int pageSize);
        Task<FileToDownloadDto> GetFileBytesAsync(string url, string userId, string ip);
        Task<bool> DeleteFileAsync(string url);
    }
}
