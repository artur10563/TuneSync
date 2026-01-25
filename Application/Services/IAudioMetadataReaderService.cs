namespace Application.Services;

public interface IAudioMetadataReaderService
{
    TimeSpan GetMp3Duration(Stream mp3Stream);
}