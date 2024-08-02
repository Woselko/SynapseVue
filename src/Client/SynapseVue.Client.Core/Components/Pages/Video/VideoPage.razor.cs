
using SynapseVue.Client.Core.Components.Pages.Photo;
using SynapseVue.Client.Core.Controllers.Media;
using SynapseVue.Shared.Dtos.Media;


namespace SynapseVue.Client.Core.Components.Pages.Video;

[Authorize]
public partial class VideoPage
{
    [AutoInject] IVideoController videoController = default!;

    private bool isLoading;
    private AddOrEditVideoModal? modal;
    private ShowVideoModal? showModal;
    private string videoNameFilter = string.Empty;

    private ConfirmMessageBox confirmMessageBox = default!;
    private BitDataGrid<VideoDto>? dataGrid;
    private BitDataGridItemsProvider<VideoDto> videosProvider = default!;
    private BitDataGridPaginationState pagination = new() { ItemsPerPage = 10 };

    string VideoNameFilter
    {
        get => videoNameFilter;
        set
        {
            videoNameFilter = value;
            _ = RefreshData();
        }
    }

    protected override async Task OnInitAsync()
    {
        PrepareGridDataProvider();

        await base.OnInitAsync();
    }

    private void PrepareGridDataProvider()
    {
        videosProvider = async req =>
        {
            isLoading = true;

            try
            {
                // https://docs.microsoft.com/en-us/odata/concepts/queryoptions-overview
                videoController.AddQueryStrings(new()
                {
                    { "$top", req.Count ?? 10 },
                    { "$skip", req.StartIndex }
                });

                if (string.IsNullOrEmpty(videoNameFilter) is false)
                {
                    videoController.AddQueryString("$filter", $"contains(Name,'{videoNameFilter}')");
                }

                if (req.GetSortByProperties().Any())
                {
                    videoController.AddQueryString("$orderby", string.Join(", ", req.GetSortByProperties().Select(p => $"{p.PropertyName} {(p.Direction == BitDataGridSortDirection.Ascending ? "asc" : "desc")}")));
                }

                var data = await videoController.GetVideos(CurrentCancellationToken);

                return BitDataGridItemsProviderResult.From(data!.Items!, (int)data!.TotalCount);
            }
            catch (Exception exp)
            {
                ExceptionHandler.Handle(exp);
                return BitDataGridItemsProviderResult.From(new List<VideoDto> { }, 0);
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

    //private async Task CreateVideo()
    //{
    //    await modal!.ShowModal(new VideoDto());
    //}

    private async Task EditVideo(VideoDto video)
    {
        await modal!.ShowModal(video);
    }

    private async Task DeleteVideo(VideoDto video)
    {
        var confirmed = await confirmMessageBox.Show(Localizer.GetString(nameof(AppStrings.AreYouSureWannaDelete), video.Name ?? string.Empty),
                                                     Localizer[nameof(AppStrings.Delete)]);

        if (confirmed)
        {
            await videoController.Delete(video.Id, CurrentCancellationToken);

            await RefreshData();
        }
    }

    private async Task PlayVideo(VideoDto video)
    {
        await showModal!.ShowModal(video);
    }
}

