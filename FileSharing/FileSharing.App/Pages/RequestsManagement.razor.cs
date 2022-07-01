using FileSharing.App.Components;
using FileSharing.App.Extensions;
using FileSharing.Common.Dtos.PaginatedTable;
using FileSharing.Common.Dtos.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace FileSharing.App.Pages
{
    public partial class RequestsManagement
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        private PaginatedTable<BlockRequestDto> _paginatedTable;
        private BlockRequestDto _blockRequestTemplate;
        private BlockRequestDto _requestToRespond;
        private string _modalRespondId = "modalRespondToRequest";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        private async Task<PaginatedListResult<BlockRequestDto>> GetRequestsAsListAsync(PaginationParameters paginationParameters)
        {
            try
            {
                var url = $"Requests/getAllRequests?page={paginationParameters.Page}&pageSize={paginationParameters.PageSize}";
                if (!string.IsNullOrWhiteSpace(paginationParameters.Search))
                    url += $"&search={paginationParameters.Search}";
                var result = await Http
                    .GetFromJsonAsync<PaginatedListResult<BlockRequestDto>>(url);
                return result;
            }
            catch (Exception ex)
            {
                await JS.ShowErrorAsync("Couldn't retreive requests, please contact the administrator");
                return new PaginatedListResult<BlockRequestDto>
                {
                    CountAll = 0,
                    Items = new()
                };
            }
        }

        async Task RespondToRequestClicked(BlockRequestDto blockRequestDto)
        {
            _requestToRespond = blockRequestDto;
            await JS.ShowModalWithIdAsync(_modalRespondId);
        }

        async Task AproveRequest()
        {
            var result = await Http.PutAsync($"Requests/AproveRequest/{_requestToRespond.Id}", null);

            if (result.IsSuccessStatusCode)
            {
                await _paginatedTable.LoadItemsFromParentAsync();
                await JS.HideModalWithIdAsync(_modalRespondId);
                await JS.ShowSuccessAsync("Request approved successfuly");
            }
            else
            {
                await JS.ShowErrorAsync($"Couldn't approve the request: {result.ReasonPhrase}");
            }
        }
    }
}
