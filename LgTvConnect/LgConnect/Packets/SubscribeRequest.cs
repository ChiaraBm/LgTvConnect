using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets;

public class SubscribeRequest<T> : PacketBase<T>
{
    [JsonPropertyName("uri")] public string Uri { get; set; }
}