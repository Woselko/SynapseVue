using SynapseVue.Client.Core.Controllers.Media;
using SynapseVue.Shared.Dtos.Media;

namespace SynapseVue.Client.Core.Components.Pages.Video;

public partial class AddOrEditVideoModal
{
    [AutoInject] IVideoController videoController = default!;

    private bool isOpen;
    private bool isLoading;
    private bool isSaving;
    private VideoDto video = new();
    private List<BitDropdownItem<string>> allCategoryList = [];
    private string selectedCategoyId = string.Empty;

    [Parameter] public EventCallback OnSave { get; set; }

    protected override async Task OnInitAsync()
    {

    }

    public async Task ShowModal(VideoDto videoToShow)
    {
        await InvokeAsync(() =>
        {
            isOpen = true;
            video = videoToShow;
            //selectedCategoyId = (video.CategoryId ?? 0).ToString();

            StateHasChanged();
        });
    }


    private async Task Save()
    {
        if (isLoading) return;

        isSaving = true;

        try
        {
            if (video.Id == 0)
            {
                await videoController.Create(video, CurrentCancellationToken);
            }
            else
            {
                await videoController.Update(video, CurrentCancellationToken);
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
