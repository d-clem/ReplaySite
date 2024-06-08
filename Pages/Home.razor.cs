using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using ReplaySite.Models;
using ReplaySite.Shared;

namespace ReplaySite.Pages
{
    public partial class Home : ComponentBase
    {
        private const int DISPLAY_LIMIT = 10;
        private int currentPage = 1;
        private int pagesNumber = 1;

        private string recherche { get; set; } = "";
        private string replayCode { get; set; }
        private string description { get; set; }
        private int weapon { get; set; }
        private int map { get; set; }
        private int mode { get; set; }
        private List<Dictionary<string, object>> replays = new ();
        private List<Weapon> weapons = new ();
        private List<Map> maps = new ();
        private List<Mode> modes = new ();

        private Dictionary<string, object> currentReplay { get; set; }

        [Inject] 
        public ILocalStorageService LocalStorage { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private bool loggedIn = false;

        private string errorMessage { get; set; }
        private string displayError { get; set; } = "none";

        protected override async Task OnInitializedAsync()
        {
            loggedIn = await LoginVerifier.Verify(LocalStorage, NavigationManager);

            using (HttpClient client = new HttpClient())
            {
                pagesNumber = await client.GetFromJsonAsync<int>(Urls.Replay + "/pages");
                replays = await client.GetFromJsonAsync<List<Dictionary<string, object>>>(Urls.Replay + "?details=true");
                weapons = await client.GetFromJsonAsync<List<Weapon>>(Urls.Weapons);
                maps = await client.GetFromJsonAsync<List<Map>>(Urls.Maps);
                modes = await client.GetFromJsonAsync<List<Mode>>(Urls.Modes);
            }
            StateHasChanged();
        }

        public async void Search(ChangeEventArgs? args)
        {
            if ( args != null && args.Value != null)
            {
                recherche = args.Value.ToString();
            }

            using (HttpClient client = new HttpClient())
            {
                replays = await client.GetFromJsonAsync<List<Dictionary<string, object>>>(
                    Urls.Replay + "?details=true&query=" + recherche + "&page=" + currentPage
                ); 
                pagesNumber = await client.GetFromJsonAsync<int>(Urls.Replay + "/pages?query=" + recherche);     

            }
            StateHasChanged();
        }

        public async void Send()
        {
            if (replayCode == null || string.IsNullOrEmpty(replayCode))
            {
                errorMessage = "Replay code is required";
                displayError = "block";
                StateHasChanged();
                return;
            }
            if (!(replayCode.Length == 16 || replayCode.Length == 19))
            {
                errorMessage = "Replay code must be 16 or 19 characters long";
                displayError = "block";
                StateHasChanged();
                return;
            }

            var userData = await LocalStorage.GetItemAsync<User>("userData");

            Console.WriteLine(userData);
            var userId = userData.Id;
            Console.WriteLine(userId);

            var replay = new Replay()
            {
                Code = replayCode,
                Date = DateTime.Now,
                Description = description,
                WeaponId = weapon,
                MapId = map,
                ModeId = mode,
                UserId = userId
            };

            string token = await LocalStorage.GetItemAsync<string>("token");

            // send the replay to the server with the token in the header
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.PostAsJsonAsync(Urls.Replay, replay);

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Created:
                    case System.Net.HttpStatusCode.OK:
                        errorMessage = null;
                        displayError = "none";
                        Search(null);
                        break;
                    case System.Net.HttpStatusCode.Unauthorized:
                        errorMessage = "You are not authorized to add a replay";
                        displayError = "block";
                        break;
                    case System.Net.HttpStatusCode.BadRequest:
                        errorMessage = await response.Content.ReadAsStringAsync();
                        displayError = "block";
                        break;

                    default:
                        errorMessage = "An unknown error occurred \n error code: " + response.StatusCode + "\n" + response.ReasonPhrase;
                        displayError = "block";
                        break;
                }
            }

            StateHasChanged();

        }    

        public void OnDeploy(int index)
        {
            currentReplay = replays[index];

            var date = currentReplay["date"];
            var newDate = DateTime.Parse(date.ToString());
            currentReplay["date"] = newDate.ToString("dd/MM/yyyy HH:mm:ss");

            StateHasChanged();

        }

        public void NextPage()
        {
            currentPage++;
            Search(null);
            StateHasChanged();
        }

        public void PreviousPage()
        {
            currentPage--;
            Search(null);
            StateHasChanged();
        }

        public async void Logout()
        {
            await LoginVerifier.Logout(LocalStorage, NavigationManager);
        }

    }
}