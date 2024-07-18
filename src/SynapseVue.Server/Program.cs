
using Hangfire;

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
        
        var app = builder.Build();

        app.UseHangfireDashboard();
        RecurringJob.AddOrUpdate("DataCollecting", () => RaspSensorLibrary.MainDataCollector.CollectData(), Cron.Minutely);

        app.ConfiureMiddlewares();

        await app.RunAsync();
    }
}
