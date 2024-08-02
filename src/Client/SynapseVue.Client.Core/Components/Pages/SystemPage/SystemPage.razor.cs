using Microsoft.AspNetCore.Components.WebAssembly.Services;
using SynapseVue.Client.Core.Controllers.System;
using SynapseVue.Shared.Dtos.System;


namespace SynapseVue.Client.Core.Components.Pages.SystemPage;

[Authorize]
public partial class SystemPage
{
    private bool isLoading = true;
    private bool isSafeMode;
    private SystemStateDto? systemState;
    [AutoInject] private ISystemController systemController = default!;
    protected override async Task OnInitAsync()
    {
        if (systemState is null)
        {
            systemState = await systemController.Get(CurrentCancellationToken);
            isSafeMode = systemState.Mode == "Safe";
        }
        await base.OnInitAsync();
        isLoading = false;
        StateHasChanged();
    }
    private async Task ChangeState()
    {
        if (systemState != null)
        {
            systemState.Mode = isSafeMode ? "Home" : "Safe";
            await systemController.Update(systemState);
            isSafeMode = !isSafeMode;
            StateHasChanged();
        }
    }

    private async Task Reboot()
    {
        await systemController.Reboot();
    }

}
