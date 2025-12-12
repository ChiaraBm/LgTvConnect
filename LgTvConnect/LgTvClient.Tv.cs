using LgTvConnect.Common;
using LgTvConnect.LgConnect;
using LgTvConnect.LgConnect.Packets.Client;
using LgTvConnect.LgConnect.Packets.Server;

namespace LgTvConnect;

public partial class LgTvClient
{
    public async Task ScreenshotAsync(Func<string, Task> callback)
    {
        ThrowIfNoReady();

        await LgConnectClient.RequestWithResultAsync<object, OneShotResponse>("ssap://tv/executeOneShot", null,
            async response => { await callback.Invoke(response.Payload.ImageUri); }
        );
    }

    public async Task<string> ScreenshotAsync()
    {
        ThrowIfNoReady();

        var tcs = new TaskCompletionSource<string>();

        await LgConnectClient.RequestWithResultAsync<object, OneShotResponse>("ssap://tv/executeOneShot", null,
            response =>
            {
                tcs.SetResult(response.Payload.ImageUri);
                return Task.CompletedTask;
            }
        );

        return await tcs.Task;
    }

    public async Task SetChannelAsync(int channel)
    {
        ThrowIfNoReady();

        await LgConnectClient.RequestAsync("ssap://tv/openChannel", new OpenChannelRequest()
        {
            ChannelNumber = channel.ToString()
        });
    }

    public async Task SwitchInputAsync(TvInput input)
    {
        ThrowIfNoReady();

        if (input == TvInput.LiveTv)
        {
            await LgConnectClient.RequestAsync("ssap://com.webos.applicationManager/launch", new LaunchAppRequest()
            {
                Id = "com.webos.app.livetv"
            });
        }
        else
        {
            var inputId = input switch
            {
                TvInput.Hdmi1 => "HDMI_1",
                TvInput.Hdmi2 => "HDMI_2",
                TvInput.Hdmi3 => "HDMI_3",
                _ => throw new ArgumentOutOfRangeException(nameof(input), input, null)
            };

            await LgConnectClient.RequestAsync("ssap://tv/switchInput", new SwitchInputRequest()
            {
                InputId = inputId
            });
        }
    }
}