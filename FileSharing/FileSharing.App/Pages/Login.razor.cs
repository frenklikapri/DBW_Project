using FileSharing.App.Services;
using FileSharing.App.Store.Identity;
using FileSharing.Common.Dtos.Authentication;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace FileSharing.App.Pages
{
    public partial class Login
    {
        private UserForAuthenticationDto _userForAuthentication = new UserForAuthenticationDto();

        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }
        
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IDispatcher Dispatcher { get; set; }

        public bool ShowAuthError { get; set; }
        public string Error { get; set; }

        public async Task ExecuteLogin()
        {
            ShowAuthError = false;

            var result = await AuthenticationService.Login(_userForAuthentication);
            if (!result.IsAuthSuccessful)
            {
                Error = result.ErrorMessage;
                ShowAuthError = true;
            }
            else
            {
                Dispatcher.Dispatch(new IdentityAction());
                NavigationManager.NavigateTo("/", true);
            }
        }
    }
}
