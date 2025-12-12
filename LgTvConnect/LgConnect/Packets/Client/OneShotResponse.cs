using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets.Client;

public class OneShotResponse
{
    [JsonPropertyName("imageUri")]
    public string ImageUri { get; set; }
}