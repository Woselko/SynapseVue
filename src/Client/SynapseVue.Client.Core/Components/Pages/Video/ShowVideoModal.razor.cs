using SynapseVue.Client.Core.Controllers.Media;
using SynapseVue.Shared.Dtos.Media;

namespace SynapseVue.Client.Core.Components.Pages.Video;

public partial class ShowVideoModal
{
    [AutoInject] IVideoController videoController = default!;

    private bool isOpen;
    private bool isLoading;
    private bool isSaving;
    private VideoDto video = new();
    private string path = string.Empty;
    private string path2 = string.Empty;
    private List<BitDropdownItem<string>> allCategoryList = [];
    private string selectedCategoyId = string.Empty;

    [Parameter] public EventCallback OnSave { get; set; }

    protected override async Task OnInitAsync()
    {

    }
    //C:\C_Sources\00SynapseVue\src\SynapseVue.Server\bin\Debug\net8.0\Media/test.avi
    public async Task ShowModal(VideoDto videoToShow)
    {
        await InvokeAsync(() =>
        {
            isOpen = true;
            video = videoToShow;
            path = video.FilePath;
            StateHasChanged();
        });

        path2 = Path.Combine(Directory.GetCurrentDirectory(), "videos", path);
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
