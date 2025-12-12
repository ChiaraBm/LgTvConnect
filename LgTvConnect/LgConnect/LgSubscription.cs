using System.Text.Json;
using LgTvConnect.LgConnect.Packets;

namespace LgTvConnect.LgConnect;

public interface ILgSubscription
{
    public Task HandleMessageAsync(string message);
}

public class LgSubscription<T> : ILgSubscription
{
    public string Id { get; private set; }
    
    private readonly Func<PacketBase<T>, Task> Handler;

    public LgSubscription(string id, Func<PacketBase<T>, Task> handler)
    {
        Id = id;
        Handler = handler;
    }

    public async Task HandleMessageAsync(string message)
    {
        var model = JsonSerializer.Deserialize<PacketBase<T>>(message)!;

        await Handler.Invoke(model);
    }
}