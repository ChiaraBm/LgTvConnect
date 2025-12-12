using LgTvConnect.LgConnect;
using LgTvConnect.LgConnect.Packets.Server;

namespace LgTvConnect;

public partial class LgTvClient
{
    public async Task ScreenOnAsync()
    {
        ThrowIfNoReady();

        await LgConnectClient.RequestAsync<object>("ssap://com.webos.service.tvpower/power/turnOnScreen", null);
    }
    
    public async Task ScreenOffAsync()
    {
        ThrowIfNoReady();

        await LgConnectClient.RequestAsync<object>("ssap://com.webos.service.tvpower/power/turnOffScreen", null);
    }
}