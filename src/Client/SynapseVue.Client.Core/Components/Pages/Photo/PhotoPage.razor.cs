using Microsoft.AspNetCore.Components.WebAssembly.Services;
using SynapseVue.Client.Core.Controllers.Media;
using SynapseVue.Shared.Dtos.Media;

namespace SynapseVue.Client.Core.Components.Pages.Photo;

[Authorize]
public partial class PhotoPage
{
    [AutoInject] IPhotoController photoController = default!;

    private bool isLoading;
    private AddOrEditPhotoModal? modal;
    private ShowPhotoModal? showModal;
    private string photoNameFilter = string.Empty;

    private ConfirmMessageBox confirmMessageBox = default!;
    private BitDataGrid<PhotoDto>? dataGrid;
    private BitDataGridItemsProvider<PhotoDto> photosProvider = default!;
    private BitDataGridPaginationState pagination = new() { ItemsPerPage = 10 };

    string PhotoNameFilter
    {
        get => photoNameFilter;
        set
        {
            photoNameFilter = value;
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
        photosProvider = async req =>
        {
            isLoading = true;

            try
            {
                // https://docs.microsoft.com/en-us/odata/concepts/queryoptions-overview
                photoController.AddQueryStrings(new()
                {
                    { "$top", req.Count ?? 10 },
                    { "$skip", req.StartIndex }
                });

                if (string.IsNullOrEmpty(photoNameFilter) is false)
                {
                    photoController.AddQueryString("$filter", $"contains(Name,'{photoNameFilter}')");
                }

                if (req.GetSortByProperties().Any())
                {
                    photoController.AddQueryString("$orderby", string.Join(", ", req.GetSortByProperties().Select(p => $"{p.PropertyName} {(p.Direction == BitDataGridSortDirection.Ascending ? "asc" : "desc")}")));
                }

                var data = await photoController.GetPhotos(CurrentCancellationToken);

                return BitDataGridItemsProviderResult.From(data!.Items!, (int)data!.TotalCount);
            }
            catch (Exception exp)
            {
                ExceptionHandler.Handle(exp);
                return BitDataGridItemsProviderResult.From(new List<PhotoDto> { }, 0);
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

    //private async Task CreatePhoto()
    //{
    //    await modal!.ShowModal(new PhotoDto());
    //}

    private async Task EditPhoto(PhotoDto photo)
    {
        await modal!.ShowModal(photo);
    }

    private async Task DeletePhoto(PhotoDto photo)
    {
        var confirmed = await confirmMessageBox.Show(Localizer.GetString(nameof(AppStrings.AreYouSureWannaDelete), photo.Name ?? string.Empty),
                                                     Localizer[nameof(AppStrings.Delete)]);

        if (confirmed)
        {
            await photoController.Delete(photo.Id, CurrentCancellationToken);

            await RefreshData();
        }
    }

    private async Task ShowPhoto(PhotoDto photo)
    {
        await showModal!.ShowModal(photo);
    }
}

