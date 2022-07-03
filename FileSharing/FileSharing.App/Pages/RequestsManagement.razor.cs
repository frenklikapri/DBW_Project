using Blazored.LocalStorage;
using FileSharing.App.Components;
using FileSharing.App.Extensions;
using FileSharing.Common.Dtos.PaginatedTable;
using FileSharing.Common.Dtos.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Net.Http.Json;

namespace FileSharing.App.Pages
{
    public partial class RequestsManagement
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        public ILocalStorageService LocalStorage { get; set; }

        [Inject]
        public IConfiguration Configuration { get; set; }

        private PaginatedTable<BlockRequestDto> _paginatedTable;
        private BlockRequestDto _blockRequestTemplate;
        private BlockRequestDto _requestToRespond;
        private string _modalRespondId = "modalRespondToRequest";
        private string _cookie = string.Empty;
        private bool _loggedIn = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await CheckIfWTCLoggedIn();
            await base.OnAfterRenderAsync(firstRender);
        }

        async Task CheckIfWTCLoggedIn()
        {
            if (await LocalStorage.ContainKeyAsync("WTCCookie"))
            {
                _cookie = await LocalStorage.GetItemAsStringAsync("WTCCookie");

                var url = "https://www.tu-chemnitz.de/informatik/DVS/blocklist/";
                Http.DefaultRequestHeaders.Add("Cookie", _cookie);
                var result = await Http.GetAsync(url + "/1234567898fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");

                _loggedIn = result.StatusCode == System.Net.HttpStatusCode.OK;
            }
        }

        async Task LoginToWTC()
        {
            var driver = new ChromeDriver(Configuration["WebDriverPath"]);
            driver.Navigate().GoToUrl("https://www.tu-chemnitz.de/informatik/DVS/blocklist/");
            driver.FindElement(By.Id("krbSubmit")).Click();

            while (true)
            {
                var cookies = driver.Manage().Cookies.AllCookies.ToList();

                if (cookies.Any(c => c.Name == "WTC_AUTHENTICATED"))
                {
                    Console.WriteLine(123);
                    var cookieStr = "";
                    foreach (var cookie in cookies)
                    {
                        cookieStr += $"{cookie.Name}={cookie.Value}";

                        if (cookies.IndexOf(cookie) == 0)
                            cookieStr += ";";
                    }
                    _cookie = cookieStr;
                    driver.Close();
                    _loggedIn = true;
                }
            }
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
