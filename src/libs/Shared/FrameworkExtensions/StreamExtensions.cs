namespace Shared.FrameworkExtensions;

public static class StreamExtensions
{
    public async static Task<(bool Successful, byte[] ByteArray)> TryGetBytesAsync(
        this Stream stream,
        bool resetStreamPositionOnRead = true,
        CancellationToken cancellationToken = default)
    {
        var readSuccessful = false;
        var byteArray = new byte[stream.Length];

        try
        {
            await stream.ReadExactlyAsync(byteArray, cancellationToken: cancellationToken);

            if (resetStreamPositionOnRead && stream.Position != 0)
                stream.Position = 0;

            readSuccessful = true;
        }
        catch (Exception)
        {
        }

        return (readSuccessful, byteArray);
    }
}
