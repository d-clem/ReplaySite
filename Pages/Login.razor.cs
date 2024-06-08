using System.Net.Http.Json;
using System.Xml.Serialization;
using ReplaySite.Models;
using ReplaySite.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Services;
using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;

namespace ReplaySite.Pages
{
    public partial class Login : ComponentBase
    {
        private string username { get; set; }
        private string password { get; set; }
        private string error { get; set; }

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

        private async Task LoginUser()
        {
            var user = new User
            {
                Login = username,
                Password = password
            };

            error = await LoginVerifier.Login(user, LocalStorage, NavigationManager);
        }

    }

}