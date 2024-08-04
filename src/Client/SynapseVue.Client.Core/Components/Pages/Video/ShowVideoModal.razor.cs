using Microsoft.AspNetCore.Http;
using Microsoft.Maui.Storage;
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
    private string videoUrl = null;
    private List<BitDropdownItem<string>> allCategoryList = [];
    private string selectedCategoyId = string.Empty;

    [Parameter] public EventCallback OnSave { get; set; }

    protected override async Task OnInitAsync()
    {

    }

    public async Task ShowModal(VideoDto videoToShow)
    {
        var access_token = await PrerenderStateService.GetValue(() => AuthTokenProvider.GetAccessTokenAsync());
        await InvokeAsync(() =>
        {
            isOpen = true;
            video = videoToShow;
            path = video.FilePath;
            path2 = Path.Combine(Directory.GetCurrentDirectory(), "videos", path);
            videoUrl = $"{Configuration.GetApiServerAddress()}api/Attachment/GetVideo?fileName={video.Name}&access_token={access_token}&file=";
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
