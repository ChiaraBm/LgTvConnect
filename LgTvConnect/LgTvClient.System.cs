using System.Net;
using System.Net.Sockets;
using LgTvConnect.Common;
using LgTvConnect.LgConnect;
using LgTvConnect.LgConnect.Packets.Server;

namespace LgTvConnect;

public partial class LgTvClient
{
    public async Task ShowToastAsync(string message)
    {
        if (LgConnectClient.State != LgClientState.Ready)
            throw new AggregateException("TV is not ready to receive such commands. Connection needs to be in Ready state");

        await LgConnectClient.RequestAsync("ssap://system.notifications/createToast", new CreateToastRequest()
        {
            Message = message
        });
    }
    
    public async Task TurnOffAsync()
    {
        if (LgConnectClient.State != LgClientState.Ready)
            throw new AggregateException("TV is not ready to receive such commands. Connection needs to be in Ready state");

        await LgConnectClient.RequestAsync<object>("ssap://system/turnOff", null);
    }
    
    public async Task WakeOnLanAsync(string macAddress)
    {
        var macBytes = macAddress.Split(':')
            .Select(s => Convert.ToByte(s, 16))
            .ToArray();

        var magicPacket = new byte[102];

        for (var i = 0; i < 6; i++)
            magicPacket[i] = 0xFF;

        for (var i = 6; i < 102; i += 6)
            Array.Copy(macBytes, 0, magicPacket, i, 6);

        using var udpClient = new UdpClient();
        udpClient.EnableBroadcast = true;
        
        await udpClient.SendAsync(
            magicPacket,
            magicPacket.Length,
            new IPEndPoint(Options.IpAddress, 9)
        );
    }
}