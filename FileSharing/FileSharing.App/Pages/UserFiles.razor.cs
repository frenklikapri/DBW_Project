using Blazored.LocalStorage;
using FileSharing.App.Extensions;
using FileSharing.Common.Dtos.Files;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

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
        private List<FileDocumentDto> _files = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (string.IsNullOrEmpty(_userId))
            {
                _userId = await LocalStorage.GetUserId();
                await RetreiveFiles();
                InvokeAsync(StateHasChanged);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        async Task RetreiveFiles()
        {
            try
            {
                _files = await Http.GetFromJsonAsync<List<FileDocumentDto>>($"Files/getUserFiles/{_userId}");
            }
            catch (Exception ex)
            {
                await JS.ShowErrorAsync("Couldn't load your files, please contact the administrator");
            }
        }
    }
}
