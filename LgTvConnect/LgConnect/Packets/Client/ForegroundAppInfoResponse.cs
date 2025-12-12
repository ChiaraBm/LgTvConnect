using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets.Client;

public class ForegroundAppInfoResponse
{
    [JsonPropertyName("appId")]
    public string AppId { get; set; }
}