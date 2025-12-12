using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets.Server;

public class PowerStateRequest
{
    [JsonPropertyName("subscribe")]
    public bool Subscribe { get; set; } = true;
}