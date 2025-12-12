using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets.Server;

public class SetMuteRequest
{
    [JsonPropertyName("mute")]
    public bool Mute { get; set; }
}