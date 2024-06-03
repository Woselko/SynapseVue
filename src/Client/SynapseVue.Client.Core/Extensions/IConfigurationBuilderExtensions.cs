using System.Reflection;

namespace Microsoft.Extensions.Configuration;

public static class IConfigurationBuilderExtensions
{
    public static void AddClientConfigurations(this IConfigurationBuilder builder)
    {
        var assembly = Assembly.Load("SynapseVue.Client.Core");
        builder.AddJsonStream(assembly.GetManifestResourceStream("SynapseVue.Client.Core.appsettings.json")!);

        if (BuildConfiguration.IsDebug())
        {
            var settings = assembly.GetManifestResourceStream("SynapseVue.Client.Core.appsettings.Development.json");
            if (settings is not null)
            {
                builder.AddJsonStream(settings);
            }
        }
        else
        {
            var settings = assembly.GetManifestResourceStream("SynapseVue.Client.Core.appsettings.Production.json");
            if (settings is not null)
            {
                builder.AddJsonStream(settings);
            }
        }
    }
}
