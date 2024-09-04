
using Hangfire;
using SynapseVue.Server.Services;

namespace SynapseVue.Server;

public static partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddClientConfigurations();

        // The following line (using the * in the URL), allows the emulators and mobile devices to access the app using the host IP address.
        if (BuildConfiguration.IsDebug() && OperatingSystem.IsWindows())
        {
            builder.WebHost.UseUrls("http://localhost:6030", "http://*:6030");
        }

        builder.ConfigureServices();
        builder.Services.AddControllers();
        var appSettings = builder.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>()!;
        AddDataCollectorFromSensors(builder);

        var app = builder.Build();



        //app.UseHangfireDashboard("/hangfire", new DashboardOptions
        //{
        //    Authorization = new[] { new DashboardAuthorizationFilter() },
        //    AppPath = appSettings.
        //})
        //.UseHangfireServer(backgroundJobServerOptions);

        //app.MapHangfireDashboardWithAuthorizationPolicy("");

        //RecurringJob.AddOrUpdate<MainDataCollector>("DataCollecting", collector => collector.CollectData(), Cron.Minutely);
        //RecurringJob.AddOrUpdate<MainVideoAnalyzer>("VideoProcessing", analyzer => analyzer.ProcessVideo(), Cron.Minutely);

        app.ConfiureMiddlewares();

        app.Use((context, next) =>
        {
            var pathBase = new PathString(context.Request.Headers["X-Forwarded-Prefix"]);
            if (pathBase != null)
                context.Request.PathBase = new PathString(pathBase.Value);
            return next();
        });
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new DashboardAuthorizationFilter(appSettings) },
            IgnoreAntiforgeryToken = true,
            AppPath = "http://localhost:6030"
        });

        //app.Use((context, next) =>
        //{
        //    var pathBase = context.Request.Headers["X-Forwarded-PathBase"];
        //    if (!string.IsNullOrEmpty(pathBase))
        //        context.Request.PathBase = new PathString(pathBase);
        //    return next();
        //});
        app.Use((context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/hangfire"))
            {
                context.Request.PathBase = new PathString(context.Request.Headers["X-Forwarded-Prefix"]);
            }
            return next();
        });

        await app.RunAsync();
    }
}
