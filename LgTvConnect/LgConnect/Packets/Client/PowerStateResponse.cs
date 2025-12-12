using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets.Client;

public class PowerStateResponse
{
    [JsonPropertyName("state")]
    public string State { get; set; }
}