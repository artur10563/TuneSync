using Application.Services;
using NAudio.Wave;

namespace Infrastructure.Services;

public class AudioMetadataReaderService : IAudioMetadataReaderService
{
    public TimeSpan GetMp3Duration(Stream mp3Stream)
    {
        mp3Stream.Position = 0;
        using var reader = new Mp3FileReader(mp3Stream);
        var total = reader.TotalTime;
        return new TimeSpan(total.Hours, total.Minutes, total.Seconds);
    }
}