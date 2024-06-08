using Microsoft.AspNetCore.Components;

namespace ReplaySite.Replays
{
    public partial class ReplayInfo : ComponentBase
    {
        [Parameter]
        #nullable enable
        public Dictionary<string, object>? Data { get; set; }

    }
}
