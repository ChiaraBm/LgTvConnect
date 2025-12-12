using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using LgTvConnect.LgConnect.Packets;
using LgTvConnect.LgConnect.Packets.Client;
using LgTvConnect.Utils;
using Microsoft.Extensions.Logging;

namespace LgTvConnect.LgConnect;

public class LgClient : IAsyncDisposable
{
    public EventSource<LgClientState> OnStateChanged { get; } = new();
    public EventSource<string> OnClientKeyReceived { get; } = new();
    public LgClientState State { get; private set; } = LgClientState.Disconnected;
    public bool IsConnected => InnerConnection.IsConnected;
    
    private int PacketCounter;
    private const string Prefix = "5d3ed79";
    
    private IAsyncDisposable? MessageSubscription;
    private IAsyncDisposable? ConnectionStateSubscription;

    private readonly ILogger Logger;
    private readonly LgConnection InnerConnection;
    private readonly ConcurrentDictionary<string, ILgSubscription> Subscriptions = new();

    private readonly JsonSerializerOptions SerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public LgClient(ILogger logger, string host, int port)
    {
        Logger = logger;
        InnerConnection = new(logger, host, port);
    }

    public async Task ConnectAsync()
    {
        if (InnerConnection is { IsConnected: true })
            throw new AggregateException("Connection is still active. Disconnect before calling connect");

        MessageSubscription = await InnerConnection.OnMessageReceived.SubscribeAsync(HandleMessageAsync);
        ConnectionStateSubscription = await InnerConnection.OnStateChanged.SubscribeAsync(HandleConnectionStateAsync);

        await InnerConnection.StartAsync();
    }

    public async Task SendPacketAsync<T>(string type, T payload)
    {
        ThrowIfNotConnected();

        var id = Prefix + Formatter.IntToStringWithLeadingZeros(PacketCounter, 5);
        PacketCounter++;

        var packet = new PacketBase<T>()
        {
            Id = id,
            Payload = payload,
            Type = type
        };

        var message = JsonSerializer.Serialize(packet, SerializerOptions);

        await InnerConnection.SendMessageAsync(message);
    }

    public async Task RequestAsync<T>(string uri, T? payload)
    {
        ThrowIfNotConnected();

        var id = Prefix + Formatter.IntToStringWithLeadingZeros(PacketCounter, 5);
        PacketCounter++;
        
        var packet = new RequestRequest<T>()
        {
            Uri = uri,
            Payload = payload!,
            Type = "request",
            Id = id
        };
        
        var message = JsonSerializer.Serialize(packet, SerializerOptions);

        await InnerConnection.SendMessageAsync(message);
    }

    public async Task SubscribeAsync<TRequest, TResult>(
        string uri,
        TRequest? payload, Func<PacketBase<TResult>, Task> callback,
        string type = "subscribe"
    )
    {
        ThrowIfNotConnected();

        var id = Prefix + Formatter.IntToStringWithLeadingZeros(PacketCounter, 5);
        PacketCounter++;

        var packet = new SubscribeRequest<TRequest>()
        {
            Id = id,
            Payload = payload!,
            Type = type,
            Uri = uri
        };

        var message = JsonSerializer.Serialize(packet, SerializerOptions);

        await InnerConnection.SendMessageAsync(message);

        Subscriptions[id] = new LgSubscription<TResult>(id, callback);
    }

    public Task UnsubscribeAsync(string id)
    {
        Subscriptions.Remove(id, out _);
        return Task.CompletedTask;
    }

    private async ValueTask HandleConnectionStateAsync(LgConnectionState state)
    {
        var clientState = state switch
        {
            LgConnectionState.Disconnected => LgClientState.Disconnected,
            LgConnectionState.Connecting => LgClientState.Connecting,
            LgConnectionState.Connected => LgClientState.Connected,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };

        await UpdateStateAsync(clientState);
    }

    private async ValueTask HandleMessageAsync(string message)
    {
        // Discard potential empty messages
        if (string.IsNullOrWhiteSpace(message))
            return;

        // Handle pairing requests without paring anything yet
        if (message.Contains("pairingType\":\"PROMPT", StringComparison.OrdinalIgnoreCase))
        {
            await UpdateStateAsync(LgClientState.Pairing);
            return;
        }
        
        try
        {
            // Parse packet base for further handling
            var packetBase = JsonSerializer.Deserialize<PacketBase>(message, SerializerOptions);

            if (packetBase == null)
            {
                Logger.LogWarning("Unable to decode message into packet base: {message}", message);
                return;
            }

            try
            {
                // Special handler for registered messages
                if (packetBase.Type == "registered")
                {
                    // Clear possible old subscriptions
                    Subscriptions.Clear();

                    await UpdateStateAsync(LgClientState.Ready);

                    // Read out client key from pairing response
                    var pairingResponse = JsonSerializer.Deserialize<PacketBase<PairingResponse>>(message, SerializerOptions);

                    if (pairingResponse == null)
                    {
                        Logger.LogError("Pairing response could not be deserialized: {message}", message);
                        return;
                    }

                    await OnClientKeyReceived.InvokeAsync(pairingResponse.Payload.ClientKey);
                    return;
                }

                // Everything else should be a subscription from here on

                // Find subscription the packet base is referring to
                var subscription = Subscriptions.GetValueOrDefault(packetBase.Id);

                if (subscription == null)
                {
                    Logger.LogDebug("Received message referring to a unknown subscription {id}: {message}", packetBase.Id, message);
                    return;
                }

                await subscription.HandleMessageAsync(message);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "An unhandled error occured while processing packet {id} from type {type}: {message}", packetBase.Id, packetBase.Type, message);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "An error occured while deserializing packet base: {message}", message);
        }
    }

    private async Task UpdateStateAsync(LgClientState state)
    {
        State = state;

        try
        {
            await OnStateChanged.InvokeAsync(state);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "An unhandled error occured on one of the OnStateChanged subscriber");
        }
    }

    private void ThrowIfNotConnected()
    {
        if (!IsConnected)
            throw new AggregateException("LG Connect connection is not available. You need to connect first");
    }

    public async ValueTask DisposeAsync()
    {
        await InnerConnection.DisposeAsync();
        
        if (MessageSubscription != null)
            await MessageSubscription.DisposeAsync();

        if (ConnectionStateSubscription != null)
            await ConnectionStateSubscription.DisposeAsync();
    }
}