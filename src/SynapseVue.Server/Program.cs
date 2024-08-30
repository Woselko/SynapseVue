
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

        app.MapHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new DashboardAuthorizationFilter(appSettings) }
        });
        //app.MapHangfireDashboardWithAuthorizationPolicy("");

        RecurringJob.AddOrUpdate<MainDataCollector>("DataCollecting", collector => collector.CollectData(), Cron.Minutely);
        RecurringJob.AddOrUpdate<MainVideoAnalyzer>("VideoProcessing", analyzer => analyzer.ProcessVideo(), Cron.Minutely);

        app.ConfiureMiddlewares();

        await app.RunAsync();
    }
}
