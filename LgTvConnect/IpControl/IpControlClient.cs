using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

namespace LgTvConnect.IpControl;

public class IpControlClient : IAsyncDisposable
{
    public bool IsConnected => TcpClient?.Connected ?? false;

    private IpControlStream? Stream;
    private TcpClient? TcpClient;

    private readonly IPEndPoint IpEndPoint;
    private readonly string EncryptionKey;

    public IpControlClient(IPEndPoint ipEndPoint, string encryptionKey)
    {
        IpEndPoint = ipEndPoint;
        EncryptionKey = encryptionKey;
    }

    public async Task ConnectAsync(CancellationToken token = default)
    {
        TcpClient = new();
        
        await TcpClient.ConnectAsync(IpEndPoint, token);
        Stream = new IpControlStream(TcpClient.GetStream(), EncryptionKey);
    }

    public async Task LaunchAppAsync(string appId, CancellationToken token = default)
    {
        ThrowIfNotConnected();
        await Stream.SendMessageAsync($"APP_LAUNCH {appId}", token);
    }

    public async Task RequestMuteStatusAsync(CancellationToken token = default)
    {
        ThrowIfNotConnected();
        await Stream.SendMessageAsync("MUTE_STATE", token);
    }

    public async Task RequestChannelAsync(CancellationToken token = default)
    {
        ThrowIfNotConnected();
        await Stream.SendMessageAsync("CURRENT_CH", token);
    }

    public async Task RequestAppAsync(CancellationToken token = default)
    {
        ThrowIfNotConnected();
        await Stream.SendMessageAsync("CURRENT_APP", token);
    }

    public async Task RequestVolumeAsync(CancellationToken token = default)
    {
        ThrowIfNotConnected();
        await Stream.SendMessageAsync("CURRENT_VOL", token);
    }

    public async Task RequestMacAddressAsync(string device, CancellationToken token = default)
    {
        ThrowIfNotConnected();
        await Stream.SendMessageAsync($"GET_MACADDRESS {device}", token);
    }

    public async Task SwitchInputAsync(IpControlInput input, CancellationToken token = default)
    {
        ThrowIfNotConnected();
        
        var id = input.GetIdentifier();
        await Stream.SendMessageAsync($"INPUT_SELECT {id}", token);
    }

    public async Task SendKeyAsync(IpControlKey key, CancellationToken token = default)
    {
        ThrowIfNotConnected();
        
        var id = key.ToCode();
        await Stream.SendMessageAsync($"KEY_ACTION {id}", token);
    }

    public async Task<string> ReceiveMessageAsync(CancellationToken token)
    {
        ThrowIfNotConnected();
        return await Stream.ReceiveMessageAsync(token);
    }

    [MemberNotNull(nameof(Stream))]
    [MemberNotNull(nameof(TcpClient))]
    private void ThrowIfNotConnected()
    {
        if (Stream == null || TcpClient is not { Connected: true })
        {
            throw new AggregateException(
                $"No stream available to send message. You need to call {nameof(ConnectAsync)} first"
            );
        }
    }

    public ValueTask DisposeAsync()
    {
        TcpClient?.Close();
        TcpClient?.Dispose();
        
        return ValueTask.CompletedTask;
    }
}