using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets;

public class PacketBase
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("payload")]
    public object? Payload { get; set; }
}

public class PacketBase<T>
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("payload")]
    public T Payload { get; set; }
}