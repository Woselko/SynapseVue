using SynapseVue.RaspCameraLibrary.Helpers;
using SynapseVue.RaspCameraLibrary.Settings.Enumerations;
using System.Text.Json.Serialization;

namespace SynapseVue.RaspCameraLibrary.Settings.Encodings;

/// <summary>
/// Png Still Settings.
/// </summary>
[JsonConverter(typeof(StillSettingsConverter<Png>))]
public class Png : StillSettings
{
    /// <summary>
    /// Will use the PNG encoding.
    /// </summary>
    public override Encoding Encoding => Encoding.Png;
}