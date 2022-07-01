using FileSharing.API.Extensions;
using FileSharing.Common.Dtos.Files;
using FileSharing.Common.Dtos.FileUpload;
using FileSharing.Common.Dtos.PaginatedTable;
using FleSharing.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net;
using System.Web;

namespace FileSharing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FilesController> _logger;
        private readonly IFileDocumentRepository _fileDocumentRepository;

        public FilesController(IWebHostEnvironment env,
            ILogger<FilesController> logger,
            IFileDocumentRepository fileDocumentRepository)
        {
            this._env = env;
            this._logger = logger;
            _fileDocumentRepository = fileDocumentRepository;
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        public async Task<ActionResult<List<FileUploadResultDto>>> PostFile(
            [FromForm] IEnumerable<IFormFile> files)
        {
            long maxFileSize = 1024 * 1024 * 10;
            var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");

            foreach (var file in files)
            {
                if (file.Length > maxFileSize)
                    return BadRequest($"The file '{file.FileName}' is bigger than 10MB");

                var bytes = await file.GetBytes();
                _fileDocumentRepository.AddFile(new FileUploadDto
                {
                    Bytes = bytes,
                    Filename = file.FileName,
                    Size = bytes.Length,
                    UserId = HttpContext.GetUserId()
                });
            }

            var results = await _fileDocumentRepository.SaveAddedFilesAsync();

            return new CreatedResult(resourcePath, results);
        }

        [HttpGet("getDocumentInfo/{url}")]
        public async Task<ActionResult<FileDocumentDto>> GetDocumentInfo(string url)
        {
            url = HttpUtility.UrlDecode(url);

            var document = await _fileDocumentRepository.GetFileInfoAsync(url);

            if (document is null)
                return NotFound();

            return Ok(document);
        }

        [HttpGet("download/{url}")]
        public async Task<ActionResult> DownloadFile(string url)
        {
            if (string.IsNullOrEmpty(Request.Headers.Referer))
                return BadRequest("Please download files using the UI app.");

            url = HttpUtility.UrlDecode(url);

            var userId = HttpContext.GetUserId();
            var ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();

            var dto = await _fileDocumentRepository.GetFileBytesAsync(url, userId, ip);

            if (dto is null)
                return NotFound();

            if (dto.DownloadLimitPassed)
            {
                return BadRequest($"Your limit of downloading 1 file per 10 minutes has passed. Last file downloaded at: {dto.LastDownloadedAt.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)}");
            }

            Stream stream = new MemoryStream(dto.Bytes);

            if (stream is null)
                return NotFound();

            return File(stream, "application/octet-stream", fileDownloadName: dto.FileName);
        }

        [Authorize]
        [HttpGet("getUserFiles/{userId}")]
        public async Task<ActionResult<PaginatedListResult<FileDocumentDto>>> GetUserFiles(string userId, int page, int pageSize,
            string? search = "")
        {
            if (search is null)
                search = string.Empty;

            var files = await _fileDocumentRepository.GetFilesUploadedByUserAsync(userId, search, page, pageSize);
            return Ok(files);
        }

        [Authorize]
        [HttpDelete("deleteFile/{url}")]
        public async Task<ActionResult> DeleteFile(string url)
        {
            url = HttpUtility.UrlDecode(url);

            var success = await _fileDocumentRepository.DeleteFileAsync(url);

            if (success)
                return Ok();
            else
                return BadRequest();
        }
    }
}
