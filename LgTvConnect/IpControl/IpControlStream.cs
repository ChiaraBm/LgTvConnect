using System.Buffers;

namespace LgTvConnect.IpControl;

public class IpControlStream
{
    private readonly Stream Stream;
    private readonly MessageEncrypter Encrypter;

    public IpControlStream(Stream stream, string encryptionKey)
    {
        Stream = stream;
        Encrypter = new MessageEncrypter(encryptionKey);
    }

    public async Task SendMessageAsync(string message, CancellationToken token = default)
    {
        var buffer = Encrypter.Encode(message);
        
        await Stream.WriteAsync(buffer, token);
        await Stream.FlushAsync(token);
    }

    public async Task<string> ReceiveMessageAsync(CancellationToken token = default)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(1024);

        var readBytes = await Stream.ReadAsync(buffer, token);
        var resizedBuffer = new byte[readBytes];
        
        Array.Copy(buffer, resizedBuffer, readBytes);
        ArrayPool<byte>.Shared.Return(buffer);

        var decryptedMessage = Encrypter.Decode(resizedBuffer);

        return decryptedMessage;
    }
}