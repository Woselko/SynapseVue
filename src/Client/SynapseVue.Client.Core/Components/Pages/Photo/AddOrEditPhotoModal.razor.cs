using SynapseVue.Client.Core.Controllers.Categories;
using SynapseVue.Client.Core.Controllers.Media;
using SynapseVue.Shared.Dtos.Media;

namespace SynapseVue.Client.Core.Components.Pages.Photo;

public partial class AddOrEditPhotoModal
{
    [AutoInject] IPhotoController photoController = default!;

    private bool isOpen;
    private bool isLoading;
    private bool isSaving;
    private PhotoDto photo = new();
    private List<BitDropdownItem<string>> allCategoryList = [];
    private string selectedCategoyId = string.Empty;

    [Parameter] public EventCallback OnSave { get; set; }

    protected override async Task OnInitAsync()
    {
    }

    public async Task ShowModal(PhotoDto photoToShow)
    {
        await InvokeAsync(() =>
        {
            isOpen = true;
            photo = photoToShow;

            StateHasChanged();
        });
    }

    private async Task Save()
    {
        if (isLoading) return;

        isSaving = true;

        try
        {
            if (photo.Id == 0)
            {
                await photoController.Create(photo, CurrentCancellationToken);
            }
            else
            {
                await photoController.Update(photo, CurrentCancellationToken);
            }

            isOpen = false;
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
