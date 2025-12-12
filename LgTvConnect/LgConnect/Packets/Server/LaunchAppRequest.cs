using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets.Server;

public class LaunchAppRequest
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
}