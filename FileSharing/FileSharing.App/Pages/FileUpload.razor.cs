using FileSharing.App.Extensions;
using FileSharing.Common.Dtos.FileUpload;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FileSharing.App.Pages
{
    public partial class FileUpload
    {
        [Inject]
        public HttpClient HttpClient { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        private List<FileContentDto> _filesToAdd = new();
        private List<FileUploadResultDto> _uploadedFiles = new();
        private bool _loading = false;

        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            if (_loading)
                return;

            long maxFileSize = 1024 * 1024 * 15;

            foreach (var file in e.GetMultipleFiles())
            {
                if (_filesToAdd.SingleOrDefault(
                    f => f.FileName == file.Name) is null)
                {
                    if (file.Size > maxFileSize)
                    {
                        await JS.ShowErrorAsync("This file exceeds the limit of 10MiB. Please upload a smaller file!", "Warning");
                        return;
                    }

                    var fileContent =
                    new StreamContent(file.OpenReadStream(maxFileSize));

                    //fileContent.Headers.ContentType =
                    //    new MediaTypeHeaderValue(file.ContentType);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    _filesToAdd.Add(new FileContentDto
                    {
                        Content = fileContent,
                        Name = "\"files\"",
                        FileName = file.Name
                    });
                }
            }
        }


        private async Task UploadFiles()
        {
            _uploadedFiles = new();
            _loading = true;
            using var content = new MultipartFormDataContent();

            foreach (var file in _filesToAdd)
            {
                content.Add(
                    content: file.Content,
                    name: file.Name,
                    fileName: file.FileName);
            }

            var response =
                await HttpClient.PostAsync("files",
                content);

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                using var responseStream =
                    await response.Content.ReadAsStreamAsync();

                var results = await JsonSerializer
                    .DeserializeAsync<List<FileUploadResultDto>>(responseStream, options);
                _uploadedFiles = results;
                _filesToAdd = new();
                await JS.ShowSuccessAsync("Files uploaded successfully");
            }
            else
            {
                await JS.ShowErrorAsync("Couldn't upload the files");
            }
            _loading = false;
        }

        private void RemoveFile(FileContentDto fileContentDto)
        {
            _filesToAdd.Remove(fileContentDto);
        }
    }

    class FileContentDto
    {
        public HttpContent Content { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
    }
}
