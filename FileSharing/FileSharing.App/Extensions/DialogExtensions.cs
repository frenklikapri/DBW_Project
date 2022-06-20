using Microsoft.JSInterop;

namespace FileSharing.App.Extensions
{
    public static class DialogExtensions
    {
        /// <summary>
        /// Shows an error toast. Uses JS
        /// </summary>
        /// <param name="jS"></param>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static async Task ShowErrorAsync(this IJSRuntime jS, string message, string title = "Error")
        {
            var module = await jS.InvokeAsync<IJSObjectReference>("import",
                "./js/dialogs.js");
            await module.InvokeAsync<string>("showError", message, title);
        }

        /// <summary>
        /// Shows a success toast
        /// </summary>
        /// <param name="jS"></param>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static async Task ShowSuccessAsync(this IJSRuntime jS, string message, string title = "Success")
        {
            var module = await jS.InvokeAsync<IJSObjectReference>("import",
                "./js/dialogs.js");
            await module.InvokeAsync<string>("showSuccess", message, title);
        }

        public static async Task ShowModalWithIdAsync(this IJSRuntime jS, string modalId, bool noCloseWithClick = false)
        {
            var module = await jS.InvokeAsync<IJSObjectReference>("import",
                "./js/dialogs.js");
            await module.InvokeAsync<string>("showModalWithId", modalId, noCloseWithClick);
        }

        public static async Task HideModalWithIdAsync(this IJSRuntime jS, string modalId)
        {
            var module = await jS.InvokeAsync<IJSObjectReference>("import",
                "./js/dialogs.js");
            await module.InvokeAsync<string>("hideModalWithId", modalId);
        }
    }
}
