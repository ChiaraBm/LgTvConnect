using LgTvConnect.LgConnect.Packets.Server;

namespace LgTvConnect;

public partial class LgTvClient
{
    public async Task OpenBrowserAsync(string url)
    {
        ThrowIfNoReady();

        await LgConnectClient.RequestAsync("ssap://system.launcher/launch", new LaunchWebBrowserRequest()
        {
            Id = "com.webos.app.browser",
            Target = url,
            Params = new()
            {
                Target = url
            }
        });
    }

    public async Task CloseWebBrowserAsync()
    {
        ThrowIfNoReady();

        await LgConnectClient.RequestAsync("ssap://system.launcher/close", new CloseAppRequest()
        {
            Id = "com.webos.app.browser"
        });
    }

    public async Task LaunchAppAsync(string appId)
    {
        ThrowIfNoReady();
        
        await LgConnectClient.RequestAsync("ssap://com.webos.applicationManager/launch", new LaunchAppRequest()
        {
            Id = appId
        });
    }
    
    public async Task CloseAppAsync(string appId)
    {
        ThrowIfNoReady();

        await LgConnectClient.RequestAsync("ssap://system.launcher/close", new CloseAppRequest()
        {
            Id = appId
        });
    }
}