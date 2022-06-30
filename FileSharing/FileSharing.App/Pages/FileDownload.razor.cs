using Blazored.LocalStorage;
using FileSharing.App.Extensions;
using FileSharing.Common.Dtos.Files;
using FileSharing.Common.Dtos.FileUpload;
using FileSharing.Common.Dtos.Requests;
using FileSharing.Common.Enums;
using FileSharing.Common.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics;
using System.Globalization;
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

        [Inject]
        public ILocalStorageService LocalStorage { get; set; }

        private const string _modalUnblockId = "modalUnBlockRequest";
        private const string _modalBlockId = "modalBlockRequest";

        private string _url = string.Empty;
        private FileDocumentDto _document = null;
        private BlockFileFormDto _unblockFileDto = new();
        private BlockFileFormDto _blockFileDto = new();
        private string _userId;
        private string _textForDownload = string.Empty;
        private long _totalRead = 0L;
        private bool _showNextDownloadTime;
        private DateTime? _lastDownloadTime;
        private Timer _nextDownloadTimer;
        private bool _loading = false;

        private int _secondsLeftToDownload
        {
            get
            {
                try
                {
                    var seconds = 600 - _lastDownloadTime.Value.SecondsBetween(DateTime.UtcNow);
                    return seconds;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (string.IsNullOrEmpty(_userId))
                {
                    _userId = await LocalStorage.GetUserId();
                    InvokeAsync(StateHasChanged);
                }
            }
            await base.OnAfterRenderAsync(firstRender);
        }

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

        public void Dispose()
        {
            _nextDownloadTimer?.Dispose();
        }

        private async Task Download()
        {
            _loading = true;
            try
            {
                //TODO change static url
                var url = $"https://localhost:7214/api/files/download/{HttpUtility.UrlEncode(_document.FileUrl)}";

                using (HttpResponseMessage response = await Http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            var str = await response.Content.ReadAsStringAsync();
                            var lastDownloadedStr = str.Substring(str.IndexOf(":") + 2).Trim();
                            var parsed = DateTime.TryParseExact(lastDownloadedStr, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None,
                                out DateTime lastDownloadedAt);
                            await JS.ShowSuccessAsync("You have passed the limit of downloading 1 file per 10 minutes.");
                            _showNextDownloadTime = true;
                            _lastDownloadTime = lastDownloadedAt;
                            _nextDownloadTimer = new Timer(async _ =>  // async void
                            {
                                await InvokeAsync(StateHasChanged);
                            }, null, 0, 1000);
                            return;
                        }
                        else
                        {
                            await JS.ShowErrorAsync("Couldn't download the file!");
                            return;
                        }
                    }
                    _showNextDownloadTime = false;
                    _nextDownloadTimer = null;
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(), fileStream = new MemoryStream())
                    {
                        var totalRead = 0L;
                        var totalReads = 0L;
                        var buffer = new byte[209715200];
                        var isMoreToRead = true;

                        do
                        {
                            var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                            if (read == 0)
                            {
                                isMoreToRead = false;
                            }
                            else
                            {
                                await fileStream.WriteAsync(buffer, 0, read);

                                totalRead += read;
                                _totalRead = totalRead;
                                totalReads += 1;

                                _textForDownload = string.Format("total bytes downloaded so far: {0:n0}", totalRead);
                                InvokeAsync(StateHasChanged);
                            }
                        }
                        while (isMoreToRead);

                        await JS.InvokeAsync<object>("BlazorDownloadFile", _document.Filename,
                            "application/octet-stream",
                            buffer);
                    }
                }
            }
            catch (Exception ex)
            {
                await JS.ShowErrorAsync("Couldn't download the file!");
            }
            _loading = false;
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
                UserId = _userId
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

        string FileSizeMB()
        {
            if (_document is null)
                return string.Empty;

            decimal mb = (decimal)_document.Size / (decimal)(1024 * 1024);
            var sizeStr = string.Format("{0:N2}MB", mb);
            return sizeStr;
        }
    }
}
