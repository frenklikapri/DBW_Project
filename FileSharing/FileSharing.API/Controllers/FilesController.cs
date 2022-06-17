using FileSharing.API.Extensions;
using FileSharing.Common.Dtos.FileUpload;
using FleSharing.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
                    UserId = ""
                });
            }

            var results = await _fileDocumentRepository.SaveAddedFilesAsync();

            return new CreatedResult(resourcePath, results);
        }
    }
}
