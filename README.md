# LgTvConnect

A C# library for controlling LG Smart TVs through multiple network-based communication protocols.

## Features

- **LG Connect WebSocket API**: Control WebOS TVs via the native WebSocket interface
- **IP Control Protocol**: Network-based control using LG's IP control protocol
- **RS-232 Over Network**: Network-based RS-232 protocol emulation (not serial hardware)
- **Unified Interface**: Single client API for all connection types
- **Auto-Pairing**: Automatically accept pairing prompts using IP Control or RS-232
- **Event-Driven Architecture**: Subscribe to TV state changes, authentication requests, and client key updates
- **Async/Await Support**: Modern asynchronous programming model

## Installation

```bash
dotnet add package LgTvConnect
```

## Supported TVs

- LG WebOS Smart TVs (2014+)
- LG Commercial Displays with IP Control
- LG TVs with network-accessible RS-232 protocol support

## Prerequisites

- Your TV and computer must be on the same network
- Enable "LG Connect Apps" in TV settings (Network → LG Connect Apps)
- For IP Control: Find your TV's IP control key in settings

## Quick Start

```csharp
using LgTvConnect;
using Microsoft.Extensions.Logging;

var logger = loggerFactory.CreateLogger<LgTvClient>();

var tv = new LgTvClient(logger, new LgTvOptions()
{
    IpAddress = IPAddress.Parse("192.168.1.100"),
    UseRs232 = true // Use RS-232 protocol over network
});

await tv.ConnectAsync();

// Handle authentication (first-time pairing)
await tv.OnAuthenticateRequested.SubscribeAsync(async () =>
{
    await tv.AuthenticateAsync("your-saved-client-key");
});

// Save the client key for future use
await tv.OnClientKeyChanged.SubscribeAsync(async key =>
{
    Console.WriteLine($"Save this key: {key}");
});

// Monitor connection state
await tv.OnStateChanged.SubscribeAsync(async state =>
{
    if (state == LgClientState.Ready)
    {
        await tv.ShowToastAsync("Connected!");
    }
});
```

## Configuration Options

```csharp
var options = new LgTvOptions()
{
    IpAddress = IPAddress.Parse("192.168.1.100"),
    
    // LG Connect WebSocket settings
    LgConnectPort = 3000,                           // Default: 3000
    
    // RS-232 over network settings
    UseRs232 = true,                                // Enable RS-232 protocol
    Rs232Port = 9761,                               // Default: 9761
    
    // IP Control settings
    UseIpControl = false,                           // Enable IP Control
    IpControlKey = "LGXXXXX",                       // Your TV's IP control key
    IpControlPort = 9761,                           // Default: 9761
    
    // Auto-pairing configuration
    AcceptSequence = LgAcceptSequence.DownEnter     // Or RightEnter
};
```

## Connection Protocols

### LG Connect (WebSocket)

The primary control method for WebOS TVs. Communicates over WebSocket on port 3000. Requires "LG Connect Apps" to be
enabled and handles authentication/pairing.

**Port**: 3000 (default)  
**Use for**: Main TV control, launching apps, media control, notifications

### RS-232 (Network Protocol)

Network-based implementation of LG's RS-232 command protocol. Despite the name, this is **not** a hardware serial
connection—it's a TCP/IP protocol on port 9761.

**Port**: 9761 (default)  
**Use for**: Auto-accepting pairing prompts, sending navigation commands, power control

### IP Control

LG's IP control protocol for commercial displays and TVs. Requires an IP control key from the TV's settings.

**Port**: 9761 (default)  
**Use for**: Auto-accepting pairing prompts, direct command control with authentication

## Auto-Pairing

The library can automatically accept pairing prompts when connecting for the first time:

```csharp
var options = new LgTvOptions()
{
    IpAddress = IPAddress.Parse("192.168.1.100"),
    UseRs232 = true, // or UseIpControl = true
    AcceptSequence = LgAcceptSequence.DownEnter // Navigates down then presses OK
};
```

**Available sequences**:

- `LgAcceptSequence.DownEnter`: Arrow Down → OK
- `LgAcceptSequence.RightEnter`: Arrow Right → OK

When the TV displays the pairing prompt, the library automatically sends the navigation commands to accept it.

## Direct Protocol Access

Execute commands directly on RS-232 or IP Control protocols:

```csharp
// Execute RS-232 command
await tv.ExecuteRs232Async(async client =>
{
    await client.SendCommandAsync(Rs232Command.Down, cancellationToken);
    await client.SendCommandAsync(Rs232Command.Enter, cancellationToken);
});

// Execute IP Control command
await tv.ExecuteIpControlAsync(async client =>
{
    await client.SendKeyAsync(IpControlKey.ArrowDown, cancellationToken);
    await client.SendKeyAsync(IpControlKey.Ok, cancellationToken);
});
```

## Authentication Flow

On first connection:

1. Call `ConnectAsync()` - establishes WebSocket connection
2. `OnAuthenticateRequested` fires when authentication is needed
3. Call `AuthenticateAsync(clientKey)` with saved key (or empty string for first time)
4. If pairing prompt appears and RS-232/IP Control is enabled, library auto-accepts
5. `OnClientKeyChanged` fires with the client key - **save this for future connections**
6. `OnStateChanged` fires with `LgClientState.Ready` when connection is complete

## Available APIs

### Power Control

```csharp
await tv.TurnOffAsync();                              // Turn TV off
await tv.WakeOnLanAsync("AA:BB:CC:DD:EE:FF");        // Wake TV using MAC address
await tv.ScreenOnAsync();                             // Turn screen on (TV stays on)
await tv.ScreenOffAsync();                            // Turn screen off (TV stays on)
```

### Volume & Audio

```csharp
await tv.SetVolumeAsync(50);                          // Set volume (0-100)
await tv.SetMuteAsync(0);                             // Mute/unmute
```

### Button Presses

```csharp
await tv.PressButtonAsync(TvButton.VolumeUp);         // Press any button
await tv.PressButtonAsync(TvButton.Enter);
```

**Available buttons**: `Back`, `ChannelUp`, `ChannelDown`, `VolumeUp`, `VolumeDown`, `Left`, `Right`, `Up`, `Down`,
`Menu`, `Enter`

**Note**: Requires `UseIpControl` or `UseRs232` to be enabled

### Inputs & Channels

```csharp
await tv.SwitchInputAsync(TvInput.Hdmi1);             // Switch HDMI input
await tv.SwitchInputAsync(TvInput.LiveTv);            // Switch to Live TV
await tv.SetChannelAsync(15);                         // Change channel
```

**Available inputs**: `Hdmi1`, `Hdmi2`, `Hdmi3`, `LiveTv`

### Screenshots

```csharp
// Get screenshot URI
string imageUri = await tv.ScreenshotAsync();

// Or use callback
await tv.ScreenshotAsync(async uri => {
    Console.WriteLine($"Screenshot: {uri}");
});
```

### Notifications

```csharp
await tv.ShowToastAsync("Hello from LgTvConnect!");
```

### Advanced

- Send raw SSAP commands via `LgConnectClient.RequestAsync()`
- Subscribe to real-time TV events
- Direct protocol command execution with `ExecuteRs232Async()` and `ExecuteIpControlAsync()`

## Network Ports

| Protocol               | Default Port | Purpose                          |
|------------------------|--------------|----------------------------------|
| LG Connect (WebSocket) | 3000         | Main TV control API              |
| RS-232 (Network)       | 9761         | RS-232 command protocol over TCP |
| IP Control             | 9761         | IP control protocol              |

**Note**: RS-232 and IP Control use the same default port but are different protocols. Enable only one at a time unless
using different port configurations.

## License

MIT

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## Credits

Based on LG's WebOS WebSocket API, RS-232 network protocol, and IP Control specifications.