using Microsoft.AspNetCore.Components.WebAssembly.Services;
using SynapseVue.Client.Core.Controllers.VideoStream;

namespace SynapseVue.Client.Core.Components.Pages.VideoStream;

[Authorize]
public partial class VideoStreamPage
{
    public bool  isLoading = true;
    protected override async Task OnInitAsync()
    {
        isLoading = false;
        await base.OnInitAsync();
    }
}
