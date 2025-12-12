using LgTvConnect.LgConnect;
using LgTvConnect.LgConnect.Packets.Server;

namespace LgTvConnect;

public partial class LgTvClient
{
    public async Task SetVolumeAsync(int volume)
    {
        if (LgConnectClient.State != LgClientState.Ready)
            throw new AggregateException("TV is not ready to receive such commands. Connection needs to be in Ready state");

        await LgConnectClient.RequestAsync("ssap://audio/setVolume", new SetVolumeRequest()
        {
            Volume = volume
        });
    }
    
    public async Task SetMuteAsync(int volume)
    {
        if (LgConnectClient.State != LgClientState.Ready)
            throw new AggregateException("TV is not ready to receive such commands. Connection needs to be in Ready state");

        await LgConnectClient.RequestAsync("ssap://audio/setVolume", new SetVolumeRequest()
        {
            Volume = volume
        });
    }
}