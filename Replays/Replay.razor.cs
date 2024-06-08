using Microsoft.AspNetCore.Components;

namespace ReplaySite.Replays
{
    public partial class Replay : ComponentBase 
    {
        [Parameter]
        public EventCallback OnDeploy { get; set; }
        [Parameter]
        public Dictionary<string, object> Data { get; set; }

        protected override void OnInitialized()
        {
        }   

        public Task Deploy()
        {
            return OnDeploy.InvokeAsync();
        }
    }
}