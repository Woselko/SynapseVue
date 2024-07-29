using SynapseVue.Server.Services;

namespace SynapseVue.Server.Controllers;

public partial class AppControllerBase : ControllerBase
{
    [AutoInject] protected AppSettings AppSettings = default!;

    [AutoInject] protected AppDbContext DbContext = default!;

    [AutoInject] protected MainMotionDetectionService MainMotionDetectionService = default!;

    [AutoInject] protected IStringLocalizer<AppStrings> Localizer = default!;
}
