using LgTvConnect.LgConnect;
using LgTvConnect.LgConnect.Packets.Server;

namespace LgTvConnect;

public partial class LgTvClient
{
    public async Task ScreenOnAsync()
    {
        if (LgConnectClient.State != LgClientState.Ready)
            throw new AggregateException("TV is not ready to receive such commands. Connection needs to be in Ready state");

        await LgConnectClient.RequestAsync<object>("ssap://com.webos.service.tvpower/power/turnOnScreen", null);
    }
    
    public async Task ScreenOffAsync()
    {
        if (LgConnectClient.State != LgClientState.Ready)
            throw new AggregateException("TV is not ready to receive such commands. Connection needs to be in Ready state");

        await LgConnectClient.RequestAsync<object>("ssap://com.webos.service.tvpower/power/turnOffScreen", null);
    }
}