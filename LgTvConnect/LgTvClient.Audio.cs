using LgTvConnect.LgConnect;
using LgTvConnect.LgConnect.Packets.Server;

namespace LgTvConnect;

public partial class LgTvClient
{
    public async Task SetVolumeAsync(int volume)
    {
        ThrowIfNoReady();

        await LgConnectClient.RequestAsync("ssap://audio/setVolume", new SetVolumeRequest()
        {
            Volume = volume
        });
    }
    
    public async Task SetMuteAsync(bool toggle)
    {
        ThrowIfNoReady();

        await LgConnectClient.RequestAsync("ssap://audio/setMute", new SetMuteRequest()
        {
            Mute = toggle
        });
    }
}