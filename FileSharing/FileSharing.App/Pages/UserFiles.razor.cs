using Blazored.LocalStorage;
using FileSharing.App.Components;
using FileSharing.App.Extensions;
using FileSharing.Common.Dtos.Files;
using FileSharing.Common.Dtos.PaginatedTable;
using FileSharing.Common.Dtos.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Web;

namespace FileSharing.App.Pages
{
    public partial class UserFiles
    {
        [Inject]
        public ILocalStorageService LocalStorage { get; set; }

        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        private string _userId;
        private PaginatedTable<FileDocumentDto> _paginatedTable;
        private FileDocumentDto _fileDocumentTemplate = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (string.IsNullOrEmpty(_userId))
            {
                _userId = await LocalStorage.GetUserId();
                if (_paginatedTable is not null)
                    await _paginatedTable.LoadItemsFromParentAsync(true);
                InvokeAsync(StateHasChanged);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        async Task DeleteFile(FileDocumentDto file)
        {
            var result = await Http.DeleteAsync($"Files/DeleteFile/{HttpUtility.UrlEncode(file.FileUrl)}");

            if (result.IsSuccessStatusCode)
            {
                await _paginatedTable.LoadItemsFromParentAsync();
                await JS.ShowSuccessAsync("File deleted successfully");
            }
            else
            {
                await JS.ShowErrorAsync("Couldn't delete the file, please contact the administrator");
            }
        }

        private async Task<PaginatedListResult<FileDocumentDto>> GetFilesAsListAsync(PaginationParameters paginationParameters)
        {
            if (string.IsNullOrEmpty(_userId))
                return new PaginatedListResult<FileDocumentDto>
                {
                    Items = new()
                };
            try
            {
                if (!string.IsNullOrWhiteSpace(paginationParameters.Search))
                    paginationParameters.Search = HttpUtility.UrlEncode(paginationParameters.Search);

                var url = $"Files/getUserFiles/{_userId}?page={paginationParameters.Page}&pageSize={paginationParameters.PageSize}";

                if (!string.IsNullOrWhiteSpace(paginationParameters.Search))
                    url += $"&search={paginationParameters.Search}";

                var result = await Http.GetFromJsonAsync<PaginatedListResult<FileDocumentDto>>(url);
                return result;
            }
            catch (Exception ex)
            {
                await JS.ShowErrorAsync("Couldn't retreive files, please contact the administrator");
                return new PaginatedListResult<FileDocumentDto>
                {
                    CountAll = 0,
                    Items = new()
                };
            }
        }

        string FileSizeMB(int size)
        {
            decimal mb = (decimal)size / (decimal)(1024 * 1024);
            var sizeStr = string.Format("{0:N2}MB", mb);
            return sizeStr;
        }
    }
}
