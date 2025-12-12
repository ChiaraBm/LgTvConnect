using System.Net;

namespace LgTvConnect;

public class LgTvOptions
{
    public IPAddress IpAddress { get; set; }

    public int LgConnectPort { get; set; } = 3000;
    
    public bool UseRs232 { get; set; } = true;
    public int Rs232Port { get; set; } = 9761;
    public bool UseIpControl { get; set; } = false;
    public string IpControlKey { get; set; } = "";
    public int IpControlPort { get; set; } = 9761;
    
    public LgAcceptSequence AcceptSequence { get; set; } = LgAcceptSequence.DownEnter;
}