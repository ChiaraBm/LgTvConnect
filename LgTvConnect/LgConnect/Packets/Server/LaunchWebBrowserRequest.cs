using System.Text.Json.Serialization;

namespace LgTvConnect.LgConnect.Packets.Server;

public class LaunchWebBrowserRequest
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("target")] public string Target { get; set; }
    
    [JsonPropertyName("params")] public ParamsData Params { get; set; }
    
    public class ParamsData
    {
        [JsonPropertyName("target")]
        public string Target { get; set; }
    }
}