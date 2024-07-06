using SynapseVue.RaspCameraLibrary.Settings;
using SynapseVue.RaspCameraLibrary.Settings.Types;
using System.Diagnostics;

namespace SynapseVue.RaspCameraLibrary;

/// <summary>
/// libcamera-vid Wrapper
/// </summary>
public class Video : Hello
{
    /// <summary>
    /// Binary name
    /// </summary>
    protected override string Executable => "libcamera-vid";

    private static Video Instance { get; } = new Video();

    /// <summary>
    /// Generate a ProcessStartInfo for libcamera-vid
    /// </summary>
    public static ProcessStartInfo CaptureStartInfo(VideoSettings settings) => Instance.StartInfo(settings);

    /// <summary>
    /// List cameras
    /// </summary>
    public static new async Task<List<Camera>?> ListCameras() => await Instance.List();
}
