namespace LgTvConnect.IpControl;

public enum IpControlInput
{
    Dtv = 0,
    Atv = 1,
    CadTv = 2,
    Catv = 3,
    Avav1 = 4,
    Component1 = 5,
    Hdmi1 = 6,
    Hdmi2 = 7,
    Hdmi3 = 8
}

public static class IpControlInputExtensions
{
    public static string GetIdentifier(this IpControlInput input)
    {
        return input switch
        {
            IpControlInput.Atv => "atv",
            IpControlInput.Avav1 => "avav1",
            IpControlInput.Catv => "catv",
            IpControlInput.Component1 => "component1",
            IpControlInput.Dtv => "dtv",
            IpControlInput.Hdmi1 => "hdmi1",
            IpControlInput.Hdmi2 => "hdmi2",
            IpControlInput.Hdmi3 => "hdmi3",
            IpControlInput.CadTv => "catdv",
            _ => throw new ArgumentOutOfRangeException(nameof(input), input, null)
        };
    }
}