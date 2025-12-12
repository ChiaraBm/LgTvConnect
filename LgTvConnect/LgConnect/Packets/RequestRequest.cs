using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets;

// I know, i know, its a weird name
public class RequestRequest<T> : PacketBase<T>
{
    [JsonPropertyName("uri")]
    public string Uri { get; set; }
}