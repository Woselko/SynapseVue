using SynapseVue.RaspCameraLibrary.Helpers;
using SynapseVue.RaspCameraLibrary.Settings.Enumerations;
using System.Text.Json.Serialization;

namespace SynapseVue.RaspCameraLibrary.Settings.Encodings;

/// <summary>
/// Yuv420 Still Settings.
/// </summary>
[JsonConverter(typeof(StillSettingsConverter<Yuv420>))]
public class Yuv420 : StillSettings
{
    /// <summary>
    /// Will use the YUV420 encoding.
    /// </summary>
    public override Encoding Encoding => Encoding.Yuv420;
}