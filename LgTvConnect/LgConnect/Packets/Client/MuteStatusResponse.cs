using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets.Client;

public class MuteStatusResponse
{
    [JsonPropertyName("mute")]
    public bool Mute { get; set; }
}