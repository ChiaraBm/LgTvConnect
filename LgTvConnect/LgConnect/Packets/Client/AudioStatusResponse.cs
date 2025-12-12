using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets.Client;

public class AudioStatusResponse
{
    [JsonPropertyName("volumeStatus")] public VolumeStatusData VolumeStatus { get; set; } = new();

    [JsonPropertyName("returnValue")]
    public bool ReturnValue { get; set; }

    [JsonPropertyName("callerId")]
    public string CallerId { get; set; }

    [JsonPropertyName("mute")]
    public bool Mute { get; set; }

    [JsonPropertyName("volume")]
    public int Volume { get; set; }
    
    public class VolumeStatusData
    {
        [JsonPropertyName("volumeOsd")]
        public string VolumeOsd { get; set; }

        [JsonPropertyName("cause")] public string Cause { get; set; } = "";

        [JsonPropertyName("mode")]
        public string Mode { get; set; }

        [JsonPropertyName("minVolume")]
        public long MinVolume { get; set; }

        [JsonPropertyName("ptcVolume")]
        public long PtcVolume { get; set; }

        [JsonPropertyName("adjustVolume")]
        public bool AdjustVolume { get; set; }

        [JsonPropertyName("activeStatus")]
        public bool ActiveStatus { get; set; }

        [JsonPropertyName("muteStatus")]
        public bool MuteStatus { get; set; }

        [JsonPropertyName("volume")]
        public long Volume { get; set; }

        [JsonPropertyName("soundOutput")]
        public string SoundOutput { get; set; }

        [JsonPropertyName("maxVolume")]
        public long MaxVolume { get; set; }
    }
}