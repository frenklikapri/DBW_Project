using Blazored.LocalStorage;
using FileSharing.App.Extensions;
using FileSharing.App.Store.Identity;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace FileSharing.App.Shared
{
    public partial class NavMenu
    {
        [Inject]
        public ILocalStorageService LocalStorage { get; set; }

        [Inject]
        public IState<IdentityState> IdentityState { get; set; }

        private bool _hasAdminRole;
        private string _userId;

        protected override void OnInitialized()
        {
            IdentityState.StateChanged += IdentityState_StateChanged;
            base.OnInitialized();
        }

        public void Dispose()
        {
            IdentityState.StateChanged -= IdentityState_StateChanged;
            base.Dispose();
        }

        private async void IdentityState_StateChanged(object? sender, EventArgs e)
        {
            await SetIdentity();
            InvokeAsync(StateHasChanged);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await SetIdentity();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task SetIdentity()
        {
            _hasAdminRole = await LocalStorage.ContainsIdentityRole("Admin");
            _userId = await LocalStorage.GetUserId();
            InvokeAsync(StateHasChanged);
        }

        private bool collapseNavMenu = true;

        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
    }
}
