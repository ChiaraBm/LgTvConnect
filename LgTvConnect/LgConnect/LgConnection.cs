using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using LgTvConnect.Utils;
using Microsoft.Extensions.Logging;

namespace LgTvConnect.LgConnect;

public class LgConnection : IAsyncDisposable
{
    public EventSource<string> OnMessageReceived { get; } = new();
    public EventSource<LgConnectionState> OnStateChanged { get; } = new();

    public bool IsConnected => WebSocket is { State: WebSocketState.Open };
    public LgConnectionState State { get; private set; } = LgConnectionState.Disconnected;

    private ClientWebSocket? WebSocket;
    private CancellationTokenSource Cts = new();

    private readonly ILogger Logger;
    private readonly string Host;
    private readonly int Port;

    private const int SendTimeout = 5;
    private const int ConnectTimeout = 5;

    public LgConnection(ILogger logger, string host, int port)
    {
        Host = host;
        Port = port;
        Logger = logger;
    }

    public Task StartAsync()
    {
        Task.Run(HandleLoopAsync);

        return Task.CompletedTask;
    }

    public async Task SendMessageAsync(string message)
    {
        if (WebSocket == null)
            throw new AggregateException("Websocket is not initialized yet. Connect first");

        if (WebSocket.State != WebSocketState.Open)
            throw new AggregateException("Websocket is not open. Unable to send message");

        var timeoutCts = new CancellationTokenSource(
            TimeSpan.FromSeconds(SendTimeout)
        );

        var buffer = Encoding.UTF8.GetBytes(message);

        try
        {
            await WebSocket.SendAsync(
                buffer,
                WebSocketMessageType.Text,
                WebSocketMessageFlags.EndOfMessage,
                timeoutCts.Token
            );
        }
        catch (OperationCanceledException)
        {
            throw new AggregateException($"Timeout of {SendTimeout} seconds while sending message reached");
        }
    }

    private async Task HandleLoopAsync()
    {
        var token = Cts.Token;

        while (!token.IsCancellationRequested)
        {
            try
            {
                await UpdateStateAsync(LgConnectionState.Disconnected);

                // Dispose old websocket if available
                WebSocket?.Dispose();
                
                WebSocket = new ClientWebSocket();

                // Options
                WebSocket.Options.SetBuffer(1016, 1016);
                WebSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(1);

                try
                {
                    var timeoutCts = new CancellationTokenSource(
                        TimeSpan.FromSeconds(ConnectTimeout)
                    );

                    await UpdateStateAsync(LgConnectionState.Connecting);

                    await WebSocket.ConnectAsync(new Uri($"ws://{Host}:{Port}"), timeoutCts.Token);
                }
                catch (OperationCanceledException)
                {
                    Logger.LogWarning("Reached timeout of {timeout} seconds while connecting", ConnectTimeout);
                    continue;
                }

                await UpdateStateAsync(LgConnectionState.Connected);

                while (!token.IsCancellationRequested && WebSocket.State == WebSocketState.Open)
                {
                    try
                    {
                        var buffer = ArrayPool<byte>.Shared.Rent(1024);

                        var receiveResult = await WebSocket.ReceiveAsync(buffer, token);
                        var resizedBuffer = new byte[receiveResult.Count];

                        Array.Copy(buffer, resizedBuffer, receiveResult.Count);

                        var message = Encoding.UTF8.GetString(resizedBuffer);

                        if (message.Contains("403 too many pairing requests", StringComparison.OrdinalIgnoreCase))
                        {
                            if (WebSocket.State == WebSocketState.Open)
                            {
                                await WebSocket.CloseAsync(
                                    WebSocketCloseStatus.NormalClosure,
                                    "Closing connection",
                                    token
                                );
                            }

                            // Waiting a few seconds to let rate limit be lifted
                            await Task.Delay(TimeSpan.FromSeconds(3), token);
                            continue;
                        }

                        if (message.Contains("403 Error!! power state", StringComparison.OrdinalIgnoreCase))
                        {
                            if (WebSocket.State == WebSocketState.Open)
                            {
                                await WebSocket.CloseAsync(
                                    WebSocketCloseStatus.NormalClosure,
                                    "Closing connection",
                                    token
                                );
                            }

                            // Waiting a second to let the TV figure out its power state
                            await Task.Delay(TimeSpan.FromSeconds(1), token);
                            continue;
                        }

                        // If we reach this point we treat the message as a regular message, so we
                        // forward it to the handler. In order to keep the connection responsive, it
                        // will run in a separate task

                        Task.Run(async () =>
                        {
                            try
                            {
                                await OnMessageReceived.InvokeAsync(message);
                            }
                            catch (Exception e)
                            {
                                Logger.LogError(e, "An unhandled exception was thrown by a subscriber of the message received event");
                            }
                        });
                    }
                    catch (OperationCanceledException)
                    {
                        // Ignored
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e, "An unhandled error occured handling incoming message");
                    }
                }

                // If the connection is still open after the connection has been canceled we
                // are disconnecting it cleanly now
                if (WebSocket.State == WebSocketState.Open)
                {
                    await WebSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Closing connection",
                        CancellationToken.None
                    );
                }
            }
            catch (OperationCanceledException)
            {
                // Ignored
            }
            catch (Exception e)
            {
                Logger.LogError(e, "An error occured while handling connection to tv");
            }
        }
    }

    private async Task UpdateStateAsync(LgConnectionState state)
    {
        State = state;
        await OnStateChanged.InvokeAsync(state);
    }

    public async ValueTask DisposeAsync()
    {
        await UpdateStateAsync(LgConnectionState.Disconnected);
        
        await Cts.CancelAsync();
        Cts.Dispose();
        WebSocket?.Dispose();
    }
}