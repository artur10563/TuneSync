namespace Application.Extensions;

public static class StreamExtensions
{
    public static long GetKilobytes(this Stream stream)
    {
        return stream.Length / 1024;
    }

    public static async Task<Stream> GetStreamFromUrlAsync(this HttpClient httpClient, string url, CancellationToken cancellationToken = default)
    {
        await using var stream = await httpClient.GetStreamAsync(url, cancellationToken);

        if (stream.CanSeek) return stream;
        
        //If stream is not seekable - copy it to new stream and rewind it
        var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Seek(0, SeekOrigin.Begin);
        
        return memoryStream;
    }
}