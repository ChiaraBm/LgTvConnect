using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LgTvConnect.Rs232;

public class Rs232Client : IAsyncDisposable
{
    public bool IsConnected => InnerClient.Connected;

    private readonly TcpClient InnerClient = new();
    private readonly IPEndPoint IpEndPoint;

    public Rs232Client(IPEndPoint ipEndPoint)
    {
        IpEndPoint = ipEndPoint;
    }

    public async Task ConnectAsync(CancellationToken token = default)
        => await InnerClient.ConnectAsync(IpEndPoint, token);

    public async Task SendCommandAsync(Rs232Command command, CancellationToken token = default)
        => await SendCommandAsync(command.GetText(), token);

    public async Task SendCommandAsync(string command, CancellationToken token = default)
    {
        const string newLineSeparator = "\r\n";

        var commandText = string.Concat(command, newLineSeparator);
        var buffer = Encoding.ASCII.GetBytes(commandText);

        var ns = InnerClient.GetStream();

        await ns.WriteAsync(buffer, token);
        await ns.FlushAsync(token);
    }
    
    public ValueTask DisposeAsync()
    {
        if(InnerClient.Connected)
            InnerClient.Close();
        
        InnerClient.Dispose();
        return ValueTask.CompletedTask;
    }
}