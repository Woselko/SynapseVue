using Microsoft.AspNetCore.Components.WebAssembly.Services;
using SynapseVue.Client.Core.Controllers.System;
using SynapseVue.Shared.Dtos.System;


namespace SynapseVue.Client.Core.Components.Pages.SystemPage;

[Authorize]
public partial class SystemPageDangerZone
{
    private bool isLoading = true;
    private bool isSafeMode;
    private SystemStateDto? systemStateDanger;
    [AutoInject] private ISystemController systemController = default!;
    protected override async Task OnInitAsync()
    {
        isLoading = true;
        PrepareGridDataProvider();
        if (systemStateDanger is null)
        {
            systemStateDanger = await systemController.GetSystemMode(CurrentCancellationToken);
            isSafeMode = systemStateDanger.Value == "Safe";
        }
        await base.OnInitAsync();
        isLoading = false;
        StateHasChanged();
    }
    private async Task ChangeState()
    {
        if (systemStateDanger != null)
        {
            systemStateDanger.Value = isSafeMode ? "Home" : "Safe";
            await systemController.Update(systemStateDanger);
            isSafeMode = !isSafeMode;
            StateHasChanged();
        }
    }

    private async Task Reboot()
    {
        await systemController.Reboot();
    }

    private AddOrEditSystemStateModal? modal;
    private string systemStateNameFilter = string.Empty;

    private ConfirmMessageBox confirmMessageBox = default!;
    private BitDataGrid<SystemStateDto>? dataGrid;
    private BitDataGridItemsProvider<SystemStateDto> systemStatesProvider = default!;
    private BitDataGridPaginationState pagination = new() { ItemsPerPage = 10 };

    string SystemStateNameFilter
    {
        get => systemStateNameFilter;
        set
        {
            systemStateNameFilter = value;
            _ = RefreshData();
        }
    }

    private void PrepareGridDataProvider()
    {
        systemStatesProvider = async req =>
        {
            isLoading = true;

            try
            {
                // https://docs.microsoft.com/en-us/odata/concepts/queryoptions-overview
                systemController.AddQueryStrings(new()
                {
                    { "$top", req.Count ?? 5 },
                    { "$skip", req.StartIndex }
                });

                if (string.IsNullOrEmpty(systemStateNameFilter) is false)
                {
                    systemController.AddQueryString("$filter", $"contains(Name,'{systemStateNameFilter}')");
                }

                if (req.GetSortByProperties().Any())
                {
                    systemController.AddQueryString("$orderby", string.Join(", ", req.GetSortByProperties().Select(p => $"{p.PropertyName} {(p.Direction == BitDataGridSortDirection.Ascending ? "asc" : "desc")}")));
                }

                var data = await systemController.GetSystemSettings(CurrentCancellationToken);

                return BitDataGridItemsProviderResult.From(data!.Items!, (int)data!.TotalCount);
            }
            catch (Exception exp)
            {
                ExceptionHandler.Handle(exp);
                return BitDataGridItemsProviderResult.From(new List<SystemStateDto> { }, 0);
            }
            finally
            {
                isLoading = false;

                StateHasChanged();
            }
        };
    }

    private async Task RefreshData()
    {
        await dataGrid!.RefreshDataAsync();
    }

    private async Task EditSystemState(SystemStateDto systemState)
    {
        await modal!.ShowModal(systemState);
    }
}
