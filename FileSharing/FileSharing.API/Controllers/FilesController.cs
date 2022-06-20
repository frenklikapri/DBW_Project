using FileSharing.API.Extensions;
using FileSharing.Common.Dtos.Files;
using FileSharing.Common.Dtos.FileUpload;
using FleSharing.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("download/{key}/{url}")]
        public async Task<ActionResult> DownloadFile(string key, string url)
        {
            url = HttpUtility.UrlDecode(url);

            //TODO save key somewhere else
            if (key != "Key123$%^&*(")
                return BadRequest();

            var bytes = await _fileDocumentRepository.GetFileBytesAsync(url);

            if (bytes is null)
                return NotFound();

            return File(bytes.Bytes, "text/plain", bytes.FileName);
        }

        [Authorize]
        [HttpGet("getUserFiles/{userId}")]
        public async Task<ActionResult<List<FileDocumentDto>>> GetUserFiles(string userId)
        {
            var files = await _fileDocumentRepository.GetFilesUploadedByUserAsync(userId);
            return Ok(files);
        }
    }
}
