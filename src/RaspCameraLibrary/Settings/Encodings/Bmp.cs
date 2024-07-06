using SynapseVue.RaspCameraLibrary.Helpers;
using SynapseVue.RaspCameraLibrary.Settings.Enumerations;
using System.Text.Json.Serialization;

namespace SynapseVue.RaspCameraLibrary.Settings.Encodings;

/// <summary>
/// Bmp Still Settings.
/// </summary>
[JsonConverter(typeof(StillSettingsConverter<Bmp>))]
public class Bmp : StillSettings
{
    /// <summary>
    /// Will use the BMP encoding.
    /// </summary>
    public override Encoding Encoding => Encoding.Bmp;
}