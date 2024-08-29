using SynapseVue.Client.Core.Controllers.System;
using SynapseVue.Shared.Dtos.System;

namespace SynapseVue.Client.Core.Components.Pages.SystemPage;

public partial class AddOrEditSystemStateModal
{
    [AutoInject] ISystemController systemStateController = default!;

    private bool isOpen;
    private bool isLoading;
    private bool isSaving;
    private SystemStateDto systemState = new();

    [Parameter] public EventCallback OnSave { get; set; }

    protected override async Task OnInitAsync()
    {
        // Initialization logic here
    }

    public async Task ShowModal(SystemStateDto systemStateToShow)
    {
        await InvokeAsync(() =>
        {
            isOpen = true;
            systemState = systemStateToShow;
            StateHasChanged();
        });
    }

    private async Task Save()
    {
        if (isLoading) return;

        isSaving = true;

        try
        {
            // Walidacja, czy systemState.Value może być liczbą całkowitą
            if (systemState.Property == "Video length" && !int.TryParse(systemState.Value, out _))
            {
                // Wywołanie błędu, jeśli walidacja się nie powiedzie
                throw new InvalidOperationException("Value must be a valid integer when Property is 'Video length'.");
            }

            // Zapis danych
            await systemStateController.Update(systemState, CurrentCancellationToken);
            isOpen = false;
        }
        catch (Exception ex)
        {
            // Wyświetlanie modala z błędem
            errorMessage = ex.Message;
            isErrorModalOpen = true;
        }
        finally
        {
            isSaving = false;
            await OnSave.InvokeAsync();
        }
    }

    private async Task OnCloseClick()
    {
        isOpen = false;
    }
}
