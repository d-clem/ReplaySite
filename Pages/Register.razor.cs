using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using ReplaySite.Models;
using ReplaySite.Shared;

namespace ReplaySite.Pages
{
    public partial class Register : ComponentBase
    {
        
        private User user = new User();
        
        private string error { get; set; }    

        private string confirmPassword { get; set; }

        [Inject] 
        public ILocalStorageService LocalStorage { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (await LoginVerifier.Verify(LocalStorage, NavigationManager))
            {
                NavigationManager.NavigateTo("/");
            }
        }

        public async void RegisterUser()
        {
            if (string.IsNullOrEmpty(user.Name))
            {
                error = "Name is required";
                return;
            }

            if (string.IsNullOrEmpty(user.Login))
            {
                error = "Login is required";
                return;
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                error = "Password is required";
                return;
            }

            if (string.IsNullOrEmpty(confirmPassword))
            {
                error = "Please confirm your password";
                return;
            }
            if (user.Password.Length < 5)
            {
                error = "Password is too short";
                return;    
            }   
            if (user.Password != confirmPassword)
            {
                error = "Passwords do not match";
                return;
            }

            error = null;

            string hash;
            byte[] hashValue = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(user.Password));
            hash = Convert.ToBase64String(hashValue);

            user.Password = hash;

            // create the user in the api
            // if the user is created, navigate to the login page

            using (var client = new HttpClient())
            {
                var response = await client.PostAsJsonAsync(Urls.User, user);

                if (response.StatusCode == System.Net.HttpStatusCode.Created
                    || response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    error = null;
                    NavigationManager.NavigateTo("/login");
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