using SynapseVue.RaspCameraLibrary.Helpers;
using SynapseVue.RaspCameraLibrary.Settings.Enumerations;
using System.Text.Json.Serialization;

namespace SynapseVue.RaspCameraLibrary.Settings.Encodings;

/// <summary>
/// Rgb Still Settings.
/// </summary>
[JsonConverter(typeof(StillSettingsConverter<Rgb>))]
public class Rgb : StillSettings
{
    /// <summary>
    /// Will use the RGB encoding.
    /// </summary>
    public override Encoding Encoding => Encoding.Rgb;
}