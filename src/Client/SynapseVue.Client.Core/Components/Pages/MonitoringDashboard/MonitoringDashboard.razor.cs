using Microsoft.AspNetCore.Components.WebAssembly.Services;
using SynapseVue.Client.Core.Controllers.Media;
using SynapseVue.Shared.Dtos.Media;

namespace SynapseVue.Client.Core.Components.Pages.MonitoringDashboard;

[Authorize]
public partial class MonitoringDashboard
{
    private bool isLoading = true;
    private string url; 
    protected override async Task OnInitAsync()
    {
        var access_token = await PrerenderStateService.GetValue(() => AuthTokenProvider.GetAccessTokenAsync());
        await PreAutorizeDashboardAsync(access_token);
        await base.OnInitAsync();
        isLoading = false;
        //url = $"{Configuration.GetApiServerAddress()}hangfire";
        url = $"{Configuration.GetApiServerAddress()}hangfire?access_token={access_token}";
        StateHasChanged();
    }
    private async Task PreAutorizeDashboardAsync(string? access_token)
    {
        var dashboardUrl = $"{Configuration.GetApiServerAddress()}hangfire?access_token={access_token}";
        //var item = navItems[3].ChildItems.First(x => x.Url == "");
        var result = await HttpClient.GetAsync(dashboardUrl);
        var json = await result.Content.ReadAsStringAsync();
    }
}

