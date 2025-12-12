using LgTvConnect.LgConnect.Packets;

namespace LgTvConnect.LgConnect;

public static class LgClientExtensions
{
    public static async Task RequestWithResultAsync<TRequest, TResponse>(
        this LgClient client,
        string uri,
        TRequest? payload,
        Func<PacketBase<TResponse>, Task> handle
        )
    {
        await client.SubscribeAsync<TRequest, TResponse>(uri, payload, async response =>
        {
            await client.UnsubscribeAsync(response.Id);

            await handle.Invoke(response);
        }, "request");
    }
}