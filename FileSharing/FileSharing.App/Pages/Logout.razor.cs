using FileSharing.App.Services;
using FileSharing.App.Store.Identity;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace FileSharing.App.Pages
{
    public partial class Logout
    {
        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }
        
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IDispatcher Dispatcher { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await AuthenticationService.Logout();
            Dispatcher.Dispatch(new IdentityAction());
            NavigationManager.NavigateTo("/", true);
        }
    }
}
