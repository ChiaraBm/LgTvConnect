using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets.Server;

public class SetVolumeRequest
{
    [JsonPropertyName("volume")]
    public int Volume { get; set; }
}