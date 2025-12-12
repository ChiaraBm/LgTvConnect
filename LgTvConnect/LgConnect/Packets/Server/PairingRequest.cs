using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets.Server;

public class PairingRequest
{
    [JsonPropertyName("forcePairing")] 
    public bool ForcePairing { get; set; } = false;

    [JsonPropertyName("pairingType")] 
    public string PairingType { get; set; } = "PROMPT";

    [JsonPropertyName("manifest")] 
    public ManifestData Manifest { get; set; } = new();

    [JsonPropertyName("client-key")] 
    public string ClientKey { get; set; }
    
    public class ManifestData
    {
        [JsonPropertyName("manifestVersion")] 
        public long ManifestVersion { get; set; } = 1;

        [JsonPropertyName("appVersion")] 
        public string AppVersion { get; set; } = "1.1";

        [JsonPropertyName("signed")] 
        public SignedData Signed { get; set; } = new();

        [JsonPropertyName("permissions")] 
        public string[] Permissions { get; set; } = new[]
        {
            "LAUNCH",
            "LAUNCH_WEBAPP",
            "APP_TO_APP",
            "CLOSE",
            "TEST_OPEN",
            "TEST_PROTECTED",
            "CONTROL_AUDIO",
            "CONTROL_DISPLAY",
            "CONTROL_INPUT_JOYSTICK",
            "CONTROL_INPUT_MEDIA_RECORDING",
            "CONTROL_INPUT_MEDIA_PLAYBACK",
            "CONTROL_INPUT_TV",
            "CONTROL_POWER",
            "READ_APP_STATUS",
            "READ_CURRENT_CHANNEL",
            "READ_INPUT_DEVICE_LIST",
            "READ_NETWORK_STATE",
            "READ_RUNNING_APPS",
            "READ_TV_CHANNEL_LIST",
            "WRITE_NOTIFICATION_TOAST",
            "READ_POWER_STATE",
            "READ_COUNTRY_INFO",
            "READ_SETTINGS",
            "CONTROL_TV_SCREEN",
            "CONTROL_TV_STANBY",
            "CONTROL_FAVORITE_GROUP",
            "CONTROL_USER_INFO",
            "CHECK_BLUETOOTH_DEVICE",
            "CONTROL_BLUETOOTH",
            "CONTROL_TIMER_INFO",
            "STB_INTERNAL_CONNECTION",
            "CONTROL_RECORDING",
            "READ_RECORDING_STATE",
            "WRITE_RECORDING_LIST",
            "READ_RECORDING_LIST",
            "READ_RECORDING_SCHEDULE",
            "WRITE_RECORDING_SCHEDULE",
            "READ_STORAGE_DEVICE_LIST",
            "READ_TV_PROGRAM_INFO",
            "CONTROL_BOX_CHANNEL",
            "READ_TV_ACR_AUTH_TOKEN",
            "READ_TV_CONTENT_STATE",
            "READ_TV_CURRENT_TIME",
            "ADD_LAUNCHER_CHANNEL",
            "SET_CHANNEL_SKIP",
            "RELEASE_CHANNEL_SKIP",
            "CONTROL_CHANNEL_BLOCK",
            "DELETE_SELECT_CHANNEL",
            "CONTROL_CHANNEL_GROUP",
            "SCAN_TV_CHANNELS",
            "CONTROL_TV_POWER",
            "CONTROL_WOL"
        };

        [JsonPropertyName("signatures")] 
        public SignatureData[] Signatures { get; set; } = new[]
        {
            new SignatureData
            {
                SignatureVersion = 1,
                SignatureSignature = "eyJhbGdvcml0aG0iOiJSU0EtU0hBMjU2Iiwia2V5SWQiOiJ0ZXN0LXNpZ25pbmctY2VydCIsInNpZ25hdHVyZVZlcnNpb24iOjF9.hrVRgjCwXVvE2OOSpDZ58hR+59aFNwYDyjQgKk3auukd7pcegmE2CzPCa0bJ0ZsRAcKkCTJrWo5iDzNhMBWRyaMOv5zWSrthlf7G128qvIlpMT0YNY+n/FaOHE73uLrS/g7swl3/qH/BGFG2Hu4RlL48eb3lLKqTt2xKHdCs6Cd4RMfJPYnzgvI4BNrFUKsjkcu+WD4OO2A27Pq1n50cMchmcaXadJhGrOqH5YmHdOCj5NSHzJYrsW0HPlpuAx/ECMeIZYDh6RMqaFM2DXzdKX9NmmyqzJ3o/0lkk/N97gfVRLW5hA29yeAwaCViZNCP8iC9aO0q9fQojoa7NQnAtw=="
            }
        };
    }

    public class SignatureData
    {
        [JsonPropertyName("signatureVersion")] 
        public long SignatureVersion { get; set; }

        [JsonPropertyName("signature")] 
        public string SignatureSignature { get; set; }
    }

    public class SignedData
    {
        [JsonPropertyName("created")]
        public string Created { get; set; } = "20140509";

        [JsonPropertyName("appId")] 
        public string AppId { get; set; } = "com.lge.test";

        [JsonPropertyName("vendorId")] 
        public string VendorId { get; set; } = "com.lge";

        [JsonPropertyName("localizedAppNames")] 
        public LocalizedAppNamesData LocalizedAppNames { get; set; } = new();

        [JsonPropertyName("localizedVendorNames")] 
        public LocalizedVendorNamesData LocalizedVendorNames { get; set; } = new();

        [JsonPropertyName("permissions")] 
        public string[] Permissions { get; set; } = new[]
        {
            "TEST_SECURE",
            "CONTROL_INPUT_TEXT",
            "CONTROL_MOUSE_AND_KEYBOARD",
            "READ_INSTALLED_APPS",
            "READ_LGE_SDX",
            "READ_NOTIFICATIONS",
            "SEARCH",
            "WRITE_SETTINGS",
            "WRITE_NOTIFICATION_ALERT",
            "CONTROL_POWER",
            "READ_CURRENT_CHANNEL",
            "READ_RUNNING_APPS",
            "READ_UPDATE_INFO",
            "UPDATE_FROM_REMOTE_APP",
            "READ_LGE_TV_INPUT_EVENTS",
            "READ_TV_CURRENT_TIME"
        };

        [JsonPropertyName("serial")] 
        public string Serial { get; set; } = "2f930e2d2cfe083771f68e4fe7bb07";
    }

    public class LocalizedAppNamesData
    {
        [JsonPropertyName("")]
        public string Empty { get; set; } = "LG Remote App";

        [JsonPropertyName("ko-KR")]
        public string KoKr { get; set; } = "리모컨 앱";

        [JsonPropertyName("zxx-XX")]
        public string ZxxXx { get; set; } = "ЛГ Rэмotэ AПП";
    }

    public class LocalizedVendorNamesData
    {
        [JsonPropertyName("")]
        public string Empty { get; set; } = "LG Electronics";
    }
}