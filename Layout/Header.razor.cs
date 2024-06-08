using System.Runtime.CompilerServices;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using ReplaySite.Shared;

namespace ReplaySite.Layout
{
    public partial class Header
    {
        [Parameter]
        public string Title { get; set; }


        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public ILocalStorageService LocalStorage { get; set; }

        private bool loggedIn = false;
        private string closer = "none";
        private bool deleting = false;
        private bool deletingAccount = false;

        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrEmpty(Title))
            {
                Title = "Replay Site";
            }

            // Check if the user is logged in
            loggedIn = await LoginVerifier.Verify(LocalStorage, NavigationManager);
        }

        public void ToogleMenu()
        {
            closer = "block";
            StateHasChanged();
        }

        public void CloseMenu()
        {
            closer = "none";
            deleting = false;
            deletingAccount = false;
            StateHasChanged();
        }

        public void Home()
        {
            NavigationManager.NavigateTo("/replays");
        }

        public async void Logout()
        {
            await LoginVerifier.Logout(LocalStorage, NavigationManager);
        }

        public void ToogleDelete()
        {
            deleting = !deleting;
            deletingAccount = false;
            StateHasChanged();
        }

        public void Delete()
        {
            deletingAccount = true;
        }

        public async void DeleteAccount()
        {
            await LoginVerifier.DeleteAccount(LocalStorage, NavigationManager, false);
        }

        public async void SetAnonymous()
        {
            await LoginVerifier.DeleteAccount(LocalStorage, NavigationManager, true);
        }
    }
}