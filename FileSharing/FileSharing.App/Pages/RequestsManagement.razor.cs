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
using System.Security.Cryptography;
using System.Text;
using System.Web;

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
        private bool _loaded = false;
        private string _blockListUrl = "";
        private bool _loading = false;

        protected override async Task OnInitializedAsync()
        {
            _blockListUrl = Configuration["BlocklistUrl"];
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await CheckIfWTCLoggedIn();

            await base.OnAfterRenderAsync(firstRender);
        }

        async Task CheckIfWTCLoggedIn()
        {
            if (await LocalStorage.ContainKeyAsync("WTCCookie"))
            {
                _cookie = await LocalStorage.GetItemAsStringAsync("WTCCookie");

                Http.DefaultRequestHeaders.Add("Cookie", _cookie);
                var result = await Http.GetAsync(_blockListUrl + "/1234567898fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");

                _loggedIn = result.StatusCode == System.Net.HttpStatusCode.OK
                    && !result.RequestMessage.RequestUri.AbsoluteUri.Contains("shibboleth");
            }

            _loaded = true;

            InvokeAsync(StateHasChanged);
        }

        async Task LoginToWTC()
        {
            var driver = new ChromeDriver(Configuration["WebDriverPath"]);
            driver.Navigate().GoToUrl("https://www.tu-chemnitz.de/informatik/DVS/blocklist/");
            driver.FindElement(By.Id("krbSubmit")).Click();

            var read = true;

            while (read)
            {
                var cookies = driver.Manage().Cookies.AllCookies.ToList();

                if (cookies.Any(c => c.Name == "WTC_AUTHENTICATED"))
                {
                    read = false;
                    var cookieStr = "";
                    foreach (var cookie in cookies)
                    {
                        cookieStr += $"{cookie.Name}={cookie.Value}";

                        if (cookies.IndexOf(cookie) == 0)
                            cookieStr += ";";
                    }
                    _cookie = cookieStr;
                    _loggedIn = true;

                    await LocalStorage.SetItemAsStringAsync("WTCCookie", _cookie);

                    driver.Close();
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
            if (_loading)
                return;

            _loading = true;

            var bytesUrl = $"{Configuration["APIBaseUrl"]}files/download/{HttpUtility.UrlEncode(_requestToRespond.Url)}";

            Http.DefaultRequestHeaders.Add("SentBy", "Application");
            using HttpResponseMessage bytesResponse = await Http.GetAsync(bytesUrl, HttpCompletionOption.ResponseHeadersRead);
            var bytes = await bytesResponse.Content.ReadAsByteArrayAsync();

            var hash = ComputeSha256Hash(bytes);
            var url = $"{_blockListUrl}{hash}";

            var successBlockList = false;

            if (_requestToRespond.RequestType == Common.Enums.BlockRequestType.Block)
            {
                var blockListResponse = await Http.PutAsync(url, null);
                successBlockList = blockListResponse.StatusCode == System.Net.HttpStatusCode.Created;
                Console.WriteLine(blockListResponse.ReasonPhrase + "|" + blockListResponse.StatusCode.ToString());
                Console.WriteLine(url);
            }
            else
            {
                var blockListResponse = await Http.DeleteAsync(url);
                successBlockList = blockListResponse.StatusCode == System.Net.HttpStatusCode.NoContent;
            }

            if (!successBlockList)
            {
                await JS.ShowErrorAsync("Couldn't send the request to Blocklist API");
                return;
            }


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
            _loading = false;
        }

        string ComputeSha256Hash(byte[] data)
        {
            // Create a SHA256   
            using var sha256Hash = SHA256.Create();

            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(data);

            // Convert byte array to a string   
            var builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
