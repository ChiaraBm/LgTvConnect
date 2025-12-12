using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets.Server;

public class SwitchInputRequest
{
    [JsonPropertyName("inputId")]
    public string InputId { get; set; }
}