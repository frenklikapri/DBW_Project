using FileSharing.App.Services;
using FileSharing.Common.Dtos.Authentication;
using Microsoft.AspNetCore.Components;

namespace FileSharing.App.Pages
{
    public partial class Registration
    {
        private UserForRegistrationDto _userForRegistration = new UserForRegistrationDto();

        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public bool ShowRegistrationErros { get; set; }
        public IEnumerable<string> Errors { get; set; }

        public async Task Register()
        {
            ShowRegistrationErros = false;

            var result = await AuthenticationService.RegisterUser(_userForRegistration);
            if (!result.IsSuccessfulRegistration)
            {
                Errors = result.Errors;
                ShowRegistrationErros = true;
            }
            else
            {
                //TODO return token from registration
                var loginResult = await AuthenticationService.Login(new UserForAuthenticationDto
                {
                    Email = _userForRegistration.Email,
                    Password = _userForRegistration.Password,
                });

                NavigationManager.NavigateTo("/");
            }
        }
    }
}
