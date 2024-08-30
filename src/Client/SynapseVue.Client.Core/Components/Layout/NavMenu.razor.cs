using SynapseVue.Client.Core.Controllers.System;
using SynapseVue.Shared.Dtos.Identity;
using SynapseVue.Shared.Dtos.System;

namespace SynapseVue.Client.Core.Components.Layout;

public partial class NavMenu
{
    private bool disposed;
    private bool isSignOutModalOpen;
    private string? profileImageUrl;
    private string? profileImageUrlBase;
    private UserDto user = new();
    private List<BitNavItem> navItems = [];
    private Action unsubscribe = default!;
    [AutoInject] private NavigationManager navManager = default!;
    [Parameter] public bool IsMenuOpen { get; set; }
    [Parameter] public EventCallback<bool> IsMenuOpenChanged { get; set; }

    protected override async Task OnInitAsync()
    {
        navItems =
        [
            new()
            {
                Text = Localizer[nameof(AppStrings.Home)],
                IconName = BitIconName.Home,
                Url = "/",
            },
            new()
            {
                Text = Localizer[nameof(AppStrings.System)],
                IconName = BitIconName.Settings,
                IsExpanded = true,
                ChildItems =
                [
                    new() {
                        Text = Localizer[nameof(AppStrings.SystemSettings)],
                        IconName = BitIconName.SettingsSecure,
                        Url = "/SystemSettings",
                    },
                    new() {
                        Text = Localizer[nameof(AppStrings.DangerZone)],
                        IconName = BitIconName.Warning,
                        Url = "/DangerZone",
                    },
                ]
            },
            new()
            {
                Text = Localizer[nameof(AppStrings.Video)],
                IconName = BitIconName.Video,
                Url = "/videostream",
            },
            new()
            {
                Text = Localizer[nameof(AppStrings.ProductCategory)],
                IconName = BitIconName.Product,
                IsExpanded = true,
                ChildItems =
                [
                    new() {
                        Text = Localizer[nameof(AppStrings.Products)],
                        IconName = BitIconName.Devices2,
                        Url = "/products",
                    },
                    new() {
                        Text = Localizer[nameof(AppStrings.Categories)],
                        IconName = BitIconName.Devices3,
                        Url = "/categories",
                    },
                    new()
                    {
                        Text = Localizer[nameof(AppStrings.Dashboard)],
                        IconName = BitIconName.History,
                        Url = "/monitoringdashboard"
                    }
                ]
            },
            new()
            {
                Text = Localizer[nameof(AppStrings.Media)],
                IconName = BitIconName.Media,
                IsExpanded = true,
                ChildItems =
                [
                    new() {
                        IconName = BitIconName.Video,
                        Text = Localizer[nameof(AppStrings.Video)],
                        Url = "/video",
                    },
                    new() {
                        IconName = BitIconName.Photo,
                        Text = Localizer[nameof(AppStrings.Photo)],
                        Url = "/photo",
                    },
                ]
            },
            new()
            {
                Text = Localizer[nameof(AppStrings.EditProfileTitle)],
                IconName = BitIconName.EditContact,
                Url = "/edit-profile",
            },
            new()
            {
                Text = Localizer[nameof(AppStrings.TermsTitle)],
                IconName = BitIconName.EntityExtraction,
                Url = "/terms",
            }
        ];

        if (AppRenderMode.IsBlazorHybrid)
        {
            // Presently, the About page is absent from the Client/Core project, rendering it inaccessible on the web platform.
            // In order to exhibit a sample page that grants direct access to native functionalities without dependence on dependency injection (DI) or publish-subscribe patterns,
            // about page is integrated within Blazor hybrid projects like Client/Maui.

            navItems.Add(new()
            {
                Text = Localizer[nameof(AppStrings.AboutTitle)],
                IconName = BitIconName.HelpMirrored,
                Url = "/about",
            });
        }

        unsubscribe = PubSubService.Subscribe(PubSubMessages.PROFILE_UPDATED, async payload =>
        {
            if (payload is null) return;

            user = (UserDto)payload;

            SetProfileImageUrl();

            await InvokeAsync(StateHasChanged);
        });
        user = (await PrerenderStateService.GetValue(() => HttpClient.GetFromJsonAsync("api/User/GetCurrentUser", AppJsonContext.Default.UserDto, CurrentCancellationToken)))!;

        var access_token = await PrerenderStateService.GetValue(() => AuthTokenProvider.GetAccessTokenAsync());

        //await PreAutorizeDashboardAsync(access_token);
        //SetDashboardUrl(access_token);
        profileImageUrlBase = $"{Configuration.GetApiServerAddress()}api/Attachment/GetProfileImage?access_token={access_token}&file=";

        SetProfileImageUrl();
    }

    //private async Task PreAutorizeDashboardAsync(string? access_token)
    //{
    //    var dashboardUrl = $"{Configuration.GetApiServerAddress()}hangfire?access_token={access_token}";
    //    //var item = navItems[3].ChildItems.First(x => x.Url == "");
    //    await HttpClient.GetAsync(dashboardUrl);
    //}

    //private void SetDashboardUrl(string access_token)
    //{
    //    var dashboardUrl = $"{Configuration.GetApiServerAddress()}hangfire?access_token={access_token}";
    //    var item = navItems[3].ChildItems.First(x => x.Url == "");
    //    item.Url = dashboardUrl;
    //    StateHasChanged();
    //}

    private void SetProfileImageUrl()
    {
        profileImageUrl = user.ProfileImageName is not null ? profileImageUrlBase + user.ProfileImageName : null;
    }

    private async Task DoSignOut()
    {
        isSignOutModalOpen = true;

        await CloseMenu();
    }

    private async Task GoToEditProfile()
    {
        await CloseMenu();
        navManager.NavigateTo("edit-profile");
    }

    private async Task HandleNavItemClick(BitNavItem item)
    {
        if (string.IsNullOrEmpty(item.Url)) return;

        await CloseMenu();
    }

    private async Task CloseMenu()
    {
        IsMenuOpen = false;
        await IsMenuOpenChanged.InvokeAsync(false);
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        await base.DisposeAsync(disposing);

        if (disposed || disposing is false) return;

        unsubscribe?.Invoke();

        disposed = true;
    }
}
