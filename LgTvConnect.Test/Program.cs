using System.Net;
using LgTvConnect;
using LgTvConnect.LgConnect;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
});

var logger = loggerFactory.CreateLogger("TV");

var tv = new LgTvClient(logger, new LgTvOptions()
{
    IpAddress = IPAddress.Parse("172.27.69.52"),
    UseRs232 = true
});

await tv.ConnectAsync();

await tv.OnAuthenticateRequested.SubscribeAsync(async () =>
{
    logger.LogInformation("Authentication requested. Sending client key");
    
    await tv.AuthenticateAsync("34398c8d7606340d55fc5cf5adad4d48");
});

await tv.OnClientKeyChanged.SubscribeAsync(async key =>
{
    logger.LogInformation("Client key changed to {key}", key);
});

await tv.OnStateChanged.SubscribeAsync(async state =>
{
    logger.LogInformation("State changed to {state}", state);

    if (state == LgClientState.Ready)
    {
        await tv.ShowToastAsync("Connection succeeded");
    }
});

Console.ReadLine();

await tv.DisposeAsync();

Console.ReadLine();