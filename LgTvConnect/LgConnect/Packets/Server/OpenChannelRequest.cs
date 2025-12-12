using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets.Server;

public class OpenChannelRequest
{
    [JsonPropertyName("channelNumber")]
    public string ChannelNumber { get; set; } 
}