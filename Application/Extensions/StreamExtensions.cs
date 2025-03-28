namespace Application.Extensions;

public static class StreamExtensions
{
    public static long GetKilobytes(this Stream stream)
    {
        return stream.Length / 1024;
    }
}