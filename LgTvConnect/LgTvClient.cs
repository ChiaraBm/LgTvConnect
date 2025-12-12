using System.Net;
using LgTvConnect.IpControl;
using LgTvConnect.LgConnect;
using LgTvConnect.LgConnect.Packets.Server;
using LgTvConnect.Rs232;
using LgTvConnect.Utils;
using Microsoft.Extensions.Logging;

namespace LgTvConnect;

public partial class LgTvClient : IAsyncDisposable
{
    public EventSource<LgClientState> OnStateChanged { get; } = new();
    public EventSource<string> OnClientKeyChanged { get; } = new();
    public EventSource OnAuthenticateRequested { get; } = new();

    public LgClient LgConnectClient { get; private set; }

    private IAsyncDisposable? ConnectStateSubscription;
    private IAsyncDisposable? ConnectKeySubscription;

    private readonly ILogger Logger;
    private readonly LgTvOptions Options;
    private readonly CancellationTokenSource Cts = new();

    public LgTvClient(ILogger logger, LgTvOptions options)
    {
        Logger = logger;
        Options = options;

        LgConnectClient = new LgClient(logger, Options.IpAddress.ToString(), Options.LgConnectPort);
    }

    public async Task ConnectAsync()
    {
        ConnectKeySubscription = await LgConnectClient.OnClientKeyReceived.SubscribeAsync(HandleKeyChangeAsync);
        ConnectStateSubscription = await LgConnectClient.OnStateChanged.SubscribeAsync(HandleStateChangeAsync);

        await LgConnectClient.ConnectAsync();
    }

    private async ValueTask HandleKeyChangeAsync(string key)
        => await OnClientKeyChanged.InvokeAsync(key);

    private async ValueTask HandleStateChangeAsync(LgClientState state)
    {
        if (state == LgClientState.Connected)
        {
            await OnAuthenticateRequested.InvokeAsync();
        }
        else if (state == LgClientState.Pairing)
        {
            // Pair popup appeared, deciding on a strategy to auto accept it
            
            var cancellationToken = Cts.Token;

            if (Options.UseIpControl)
            {
                await ExecuteIpControlAsync(async client =>
                {
                    switch (Options.AcceptSequence)
                    {
                        case LgAcceptSequence.DownEnter:

                            await client.SendKeyAsync(IpControlKey.ArrowDown, cancellationToken);
                            await Task.Delay(2000, cancellationToken);
                            await client.SendKeyAsync(IpControlKey.Ok, cancellationToken);

                            break;

                        case LgAcceptSequence.RightEnter:

                            await client.SendKeyAsync(IpControlKey.ArrowRight, cancellationToken);
                            await Task.Delay(2000, cancellationToken);
                            await client.SendKeyAsync(IpControlKey.Ok, cancellationToken);

                            break;
                    }
                });
            }
            else if (Options.UseRs232)
            {
                await ExecuteRs232Async(async client =>
                {
                    switch (Options.AcceptSequence)
                    {
                        case LgAcceptSequence.DownEnter:

                            await client.SendCommandAsync(Rs232Command.Down, cancellationToken);
                            await Task.Delay(2000, cancellationToken);
                            await client.SendCommandAsync(Rs232Command.Enter, cancellationToken);

                            break;

                        case LgAcceptSequence.RightEnter:

                            await client.SendCommandAsync(Rs232Command.Right, cancellationToken);
                            await Task.Delay(2000, cancellationToken);
                            await client.SendCommandAsync(Rs232Command.Enter, cancellationToken);

                            break;
                    }
                });
            }
        }

        await OnStateChanged.InvokeAsync(state);
    }

    public async Task ExecuteRs232Async(Func<Rs232Client, Task> callback)
    {
        await using var client = new Rs232Client(
            new IPEndPoint(Options.IpAddress, Options.Rs232Port)
        );

        await client.ConnectAsync();

        await callback.Invoke(client);
    }

    public async Task ExecuteIpControlAsync(Func<IpControlClient, Task> callback)
    {
        await using var client = new IpControlClient(
            new IPEndPoint(Options.IpAddress, Options.IpControlPort),
            Options.IpControlKey
        );

        await client.ConnectAsync();

        await callback.Invoke(client);
    }

    public async Task AuthenticateAsync(string key = "")
    {
        var request = new PairingRequest
        {
            ClientKey = key
        };

        await LgConnectClient.SendPacketAsync("register", request);
    }

    public async ValueTask DisposeAsync()
    {
        if (ConnectStateSubscription != null)
            await ConnectStateSubscription.DisposeAsync();

        if (ConnectKeySubscription != null)
            await ConnectKeySubscription.DisposeAsync();
        
        await LgConnectClient.DisposeAsync();
        Cts.Dispose();
    }
}