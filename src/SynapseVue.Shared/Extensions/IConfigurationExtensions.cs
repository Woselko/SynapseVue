using Microsoft.Maui.Devices;

namespace Microsoft.Extensions.Configuration;
public static class IConfigurationExtensions
{
    public static string GetApiServerAddress(this IConfiguration configuration)
    {
        //var apiServerAddress = configuration.GetValue("ApiServerAddress", defaultValue: "/")!COMMENTED_WOSELKO;
        var windowsTestApiServerAddress = configuration.GetValue("WindowsTestApiServerAddress", defaultValue: "/")!;
        var androidTestApiServerAddress = configuration.GetValue("AndroidTestApiServerAddress", defaultValue: "/")!;
        var apiServerAddress = DeviceInfo.Platform == DevicePlatform.Android ? androidTestApiServerAddress : windowsTestApiServerAddress;
        return Uri.TryCreate(apiServerAddress, UriKind.RelativeOrAbsolute, out _) ? apiServerAddress : throw new InvalidOperationException($"Api server address {apiServerAddress} is invalid");
    }
}
