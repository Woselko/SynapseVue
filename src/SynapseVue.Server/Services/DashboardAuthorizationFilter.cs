using Hangfire.Dashboard;

namespace SynapseVue.Server.Services;

public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        //HttpContext.Request.Cookies.TryGetValue("access_token", out string? token);

        httpContext.Request.Cookies.TryGetValue("access_token", out string? token);

        return httpContext.User.Identity.IsAuthenticated && httpContext.User.IsInRole("HangfireAdmin");
    }
}
