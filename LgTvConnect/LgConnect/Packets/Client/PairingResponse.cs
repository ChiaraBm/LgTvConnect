using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets.Client;

public class PairingResponse
{
    [JsonPropertyName("client-key")] public string ClientKey { get; set; }
}