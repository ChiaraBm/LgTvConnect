namespace LgTvConnect.Rs232;

public enum Rs232Command
{
    ChannelUp = 0,
    ChannelDown = 1,
    VolumeUp = 2,
    VolumeDown = 3,
    Left = 4,
    Right = 5,
    Up = 6,
    Down = 7,
    Menu = 8,
    Enter = 9,
    Back = 10
}

public static class Rs232CommandExtensions
{
    public static string GetText(this Rs232Command command)
    {
        return command switch
        {
            Rs232Command.ChannelUp => "mc 1 00",
            Rs232Command.ChannelDown => "mc 1 01",
            Rs232Command.VolumeUp => "mc 1 02",
            Rs232Command.VolumeDown => "mc 1 03",
            Rs232Command.Left => "mc 1 07",
            Rs232Command.Right => "mc 1 06",
            Rs232Command.Up => "mc 1 40",
            Rs232Command.Down => "mc 1 41",
            Rs232Command.Menu => "mc 1 43",
            Rs232Command.Enter => "mc 1 44",
            Rs232Command.Back => "mc 1 28",
            _ => throw new ArgumentOutOfRangeException(nameof(command), command, null)
        };
    }
}