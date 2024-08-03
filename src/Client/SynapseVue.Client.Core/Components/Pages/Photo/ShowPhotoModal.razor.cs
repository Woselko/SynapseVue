using SynapseVue.Client.Core.Controllers.Categories;
using SynapseVue.Client.Core.Controllers.Media;
using SynapseVue.Shared.Dtos.Media;

namespace SynapseVue.Client.Core.Components.Pages.Photo;

public partial class ShowPhotoModal
{
    [AutoInject] IPhotoController photoController = default!;

    private bool isOpen;
    private bool isLoading;
    private bool isSaving;
    private string bytes;
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
            bytes = Convert.ToBase64String(photo.Data);
            StateHasChanged();
        });
    }

    private async Task Save()
    {
        if (isLoading) return;

    }

    private async Task OnCloseClick()
    {
        isOpen = false;
    }
}
