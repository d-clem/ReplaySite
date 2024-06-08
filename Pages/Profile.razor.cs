using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using ReplaySite.Models;
using ReplaySite.Shared;

namespace ReplaySite.Pages
{
    public partial class Profile : ComponentBase
    {
        [Inject]
        public ILocalStorageService LocalStorage { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private User userData { get; set; } 
        private string error { get; set; }
        private string confirmation { get; set; }
        private string oldPassword { get; set; }
        private string newPassword { get; set; }
        private string confirmPassword { get; set; }

        protected override async void OnInitialized()
        {
            if (! await LoginVerifier.Verify(LocalStorage, NavigationManager))
            {
                NavigationManager.NavigateTo("/login");
            }

            userData = await LocalStorage.GetItemAsync<User>("userData");

            Console.WriteLine(userData);

            StateHasChanged();

        }

        public async Task RegisterUser()
        {
            if (string.IsNullOrEmpty(userData.Name))
            {
                error = "Name is required";
                return;
            }

            if (string.IsNullOrEmpty(userData.Login))
            {
                error = "Login is required";
                return;
            }

            if (!string.IsNullOrEmpty(newPassword))
            {
                if (newPassword != confirmPassword)
                {
                    error = "New password and confirmation do not match";
                    return;
                }

                userData.Password = newPassword;
            }

            if (! await LoginVerifier.VerifyPassword(oldPassword))
            {
                error = "Old password is incorrect";
                return;
            }

            error = null;

            // create the user in the api
            // if the user is created, navigate to the login page

            using (var client = new HttpClient())
            {
                var response = await client.PutAsJsonAsync(Urls.User, userData);

                if (response.StatusCode == System.Net.HttpStatusCode.Created
                    || response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    error = null;
                    confirmation = "User updated successfully";
                    await LocalStorage.SetItemAsync("userData", userData);

                    newPassword = null;
                    oldPassword = null;
                    confirmPassword = null;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    error = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine(response.StatusCode);
                    error = "An error occurred \n\n"
                            + await response.Content.ReadAsStringAsync();
                }

                StateHasChanged();
            }

        }

    }
    
}