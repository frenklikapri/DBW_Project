using FileSharing.Common.Dtos.FileUpload;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FileSharing.App.Pages
{
    public partial class FileUpload
    {
        [Inject]
        public HttpClient HttpClient { get; set; }

        private List<FileContentDto> _filesToAdd = new();
        private List<FileUploadResultDto> _uploadedFiles = new();

        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            //TODO implement maxfilesize
            long maxFileSize = 1024 * 1024 * 15;

            foreach (var file in e.GetMultipleFiles())
            {
                if (_filesToAdd.SingleOrDefault(
                    f => f.FileName == file.Name) is null)
                {
                    var fileContent =
                    new StreamContent(file.OpenReadStream(maxFileSize));

                    fileContent.Headers.ContentType =
                        new MediaTypeHeaderValue(file.ContentType);

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
            }
        }
    }

    class FileContentDto
    {
        public HttpContent Content { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
    }
}
