using FileSharing.App.Extensions;
using FileSharing.Common.Dtos.Files;
using FileSharing.Common.Dtos.Requests;
using FileSharing.Common.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Web;

namespace FileSharing.App.Pages
{
    public partial class FileDownload
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        private const string _modalUnblockId = "modalUnBlockRequest";
        private const string _modalBlockId = "modalBlockRequest";

        private string _url = string.Empty;
        private FileDocumentDto _document = null;
        private BlockFileFormDto _unblockFileDto = new();
        private BlockFileFormDto _blockFileDto = new();

        private async Task GetInfo()
        {
            try
            {
                var info = await Http.GetFromJsonAsync<FileDocumentDto>($"Files/getDocumentInfo/{HttpUtility.UrlEncode(_url)}");
                _document = info;
            }
            catch (Exception ex)
            {
                await JS.ShowErrorAsync("Couldn't get the document info, please check the URL!", "Invalid URL");
            }
        }

        private async Task Download()
        {
            try
            {
                //TODO change static url
                await JS.InvokeAsync<object>("downloadURI",
                    $"https://localhost:7214/api/files/download/Key123$%^&*(/{HttpUtility.UrlEncode(_document.FileUrl)}",
                    _document.Filename);
            }
            catch (Exception ex)
            {
                await JS.ShowErrorAsync("Couldn't download the file!");
            }
        }

        async Task UrlOnInput(ChangeEventArgs e)
        {
            _url = e.Value.ToString();
        }

        async Task SendUnblockRequest()
        {
            var result = await SendRequestAsync(BlockRequestType.Unblock);

            if (result.IsSuccessStatusCode)
            {
                await JS.ShowSuccessAsync("The request was sent successfully");
                await JS.HideModalWithIdAsync(_modalUnblockId);
                _unblockFileDto = new();
            }
            else
            {
                await JS.ShowErrorAsync(result.ReasonPhrase);
            }
        }

        private async Task<HttpResponseMessage?> SendRequestAsync(BlockRequestType blockRequestType)
        {
            var dto = new AddBlockRequestDto
            {
                BlockRequestType = blockRequestType,
                FileDocumentId = _document.Id.ToString(),
                Reason = blockRequestType == BlockRequestType.Unblock ? _unblockFileDto.Reason : _blockFileDto.Reason,
                //TODO get userid
                UserId = ""
            };
            var result = await Http.PostAsJsonAsync("Requests", dto);
            return result;
        }

        async Task RequestUnblockClicked()
        {
            await JS.ShowModalWithIdAsync(_modalUnblockId);
        }

        async Task SendBlockRequest()
        {
            var result = await SendRequestAsync(BlockRequestType.Block);

            if (result.IsSuccessStatusCode)
            {
                await JS.ShowSuccessAsync("The request was sent successfully");
                await JS.HideModalWithIdAsync(_modalBlockId);
                _blockFileDto = new();
            }
            else
            {
                await JS.ShowErrorAsync(result.ReasonPhrase);
            }
        }

        async Task RequestBlockClicked()
        {
            await JS.ShowModalWithIdAsync(_modalBlockId);
        }
    }
}
