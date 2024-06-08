using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using ReplaySite.Models;

namespace ReplaySite.Shared
{
    public class LoginVerifier
    {
        
        public static async Task<string> Login(User user, ILocalStorageService LocalStorage, NavigationManager NavigationManager)
        {
            string error = null;
            using (var client = new HttpClient())
            {
                var response = await client.PostAsJsonAsync(Urls.Login, user);

                if (response.IsSuccessStatusCode)
                {
                    var userData = await response.Content.ReadFromJsonAsync<User>();
                    
                    await LocalStorage.SetItemAsync("token", userData.LastToken);

                    await LocalStorage.SetItemAsync("userData", userData);

                    // Store the current time as a timestamp
                    await LocalStorage.SetItemAsync("timestamp", DateTime.UtcNow);

                    NavigationManager.NavigateTo("/");
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        error = "Invalid username or password";
                    }
                    else
                    {
                        error = await response.Content.ReadAsStringAsync();
                    }
                }
            }

            return error;
        }

        public static async Task<bool> Verify(ILocalStorageService LocalStorage, NavigationManager NavigationManager)
        {

            var token = await LocalStorage.GetItemAsync<string>("token");
            var timestamp = await LocalStorage.GetItemAsync<DateTime>("timestamp");

            if (token == null )
            {
                return false;
            }

            if (DateTime.UtcNow - timestamp > TimeSpan.FromHours(5))
            {
                return false;
            }

            return true;
        }

        public static async Task<bool> VerifyPassword(string toCheck)
        {
            if (string.IsNullOrWhiteSpace(toCheck))
            {
                return false;
            }

            string hash;
            byte[] hashValue = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(toCheck));
            hash = Convert.ToBase64String(hashValue);

            using (var client = new HttpClient())
            {
                var response = await client.PostAsJsonAsync(Urls.Login + "/check", hash);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static async Task Logout(ILocalStorageService LocalStorage, NavigationManager NavigationManager)
        {
            await LocalStorage.RemoveItemAsync("token");
            await LocalStorage.RemoveItemAsync("timestamp");
            NavigationManager.NavigateTo("/login");
        }

        public static async Task DeleteAccount(ILocalStorageService localStorage, NavigationManager navigationManager, bool anonymous)
        {
            var token = await localStorage.GetItemAsync<string>("token");
            var userData = await localStorage.GetItemAsync<User>("userData");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = null;

                string url;

                if (anonymous)
                {
                    url = Urls.User + "/anonymous";
                }
                else
                {
                    url = Urls.User;
                }

                Console.WriteLine(url);

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(url),
                    Content = new StringContent(JsonSerializer.Serialize(userData), Encoding.UTF8, "application/json")
                };

                response = await client.SendAsync(request);
                

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    await localStorage.RemoveItemAsync("token");
                    await localStorage.RemoveItemAsync("timestamp");
                    await localStorage.RemoveItemAsync("userData");
                    navigationManager.NavigateTo("/login");
                }
                else
                {
                    Console.WriteLine(response.StatusCode);
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
            }
            
        }
    }

}