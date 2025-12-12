using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets.Client;

public class CurrentChannelResponse
{
    [JsonPropertyName("channelId")] public string ChannelId { get; set; }

    [JsonPropertyName("physicalNumber")] public long PhysicalNumber { get; set; }

    [JsonPropertyName("channelTypeName")] public string ChannelTypeName { get; set; }

    [JsonPropertyName("programNumber")] public long ProgramNumber { get; set; }


    [JsonPropertyName("channelModeName")] public string ChannelModeName { get; set; }

    [JsonPropertyName("channelNumber")] public string ChannelNumber { get; set; }

    [JsonPropertyName("isChannelChanged")] public bool IsChannelChanged { get; set; }

    [JsonPropertyName("channelTypeId")] public long ChannelTypeId { get; set; }


    [JsonPropertyName("channelName")] public string ChannelName { get; set; }

    [JsonPropertyName("channelModeId")] public long ChannelModeId { get; set; }
}