using LgTvConnect.Common;
using LgTvConnect.IpControl;
using LgTvConnect.Rs232;

namespace LgTvConnect;

public partial class LgTvClient
{
    public async Task PressButtonAsync(TvButton button)
    {
        if (Options.UseIpControl)
        {
            await ExecuteIpControlAsync(async client =>
            {
                var key = button switch
                {
                    TvButton.Back => IpControlKey.ReturnBack,
                    TvButton.ChannelUp => IpControlKey.ChannelUp,
                    TvButton.ChannelDown => IpControlKey.ChannelDown,
                    TvButton.VolumeUp => IpControlKey.VolumeUp,
                    TvButton.VolumeDown => IpControlKey.VolumeDown,
                    TvButton.Left => IpControlKey.ArrowLeft,
                    TvButton.Right => IpControlKey.ArrowRight,
                    TvButton.Up => IpControlKey.ArrowUp,
                    TvButton.Down => IpControlKey.ArrowDown,
                    TvButton.Menu => IpControlKey.SettingMenu,
                    TvButton.Enter => IpControlKey.Ok,
                    _ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
                };

                await client.SendKeyAsync(key);
            });
        }

        if (Options.UseRs232)
        {
            await ExecuteRs232Async(async client =>
            {
                var command = button switch
                {
                    TvButton.Back => Rs232Command.Back,
                    TvButton.ChannelUp => Rs232Command.ChannelUp,
                    TvButton.ChannelDown => Rs232Command.ChannelDown,
                    TvButton.VolumeUp => Rs232Command.VolumeUp,
                    TvButton.VolumeDown => Rs232Command.VolumeDown,
                    TvButton.Left => Rs232Command.Left,
                    TvButton.Right => Rs232Command.Right,
                    TvButton.Up => Rs232Command.Up,
                    TvButton.Down => Rs232Command.Down,
                    TvButton.Menu => Rs232Command.Menu,
                    TvButton.Enter => Rs232Command.Enter,
                    _ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
                };

                await client.SendCommandAsync(command);
            });
        }
    }
}