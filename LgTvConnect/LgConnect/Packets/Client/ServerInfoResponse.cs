using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets.Client;

public class ServerInfoResponse
{
    [JsonPropertyName("modelName")]
    public string ModelName { get; set; }

    [JsonPropertyName("serialNumber")]
    public string SerialNumber { get; set; }
}