using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets.Server;

public class CloseAppRequest
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
}