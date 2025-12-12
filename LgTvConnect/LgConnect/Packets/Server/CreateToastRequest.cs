using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets.Server;

public class CreateToastRequest
{
    [JsonPropertyName("message")]
    public string Message { get; set; }
}