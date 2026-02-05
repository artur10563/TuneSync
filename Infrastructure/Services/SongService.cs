using Application.Extensions;
using Application.Repositories.Shared;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.Primitives;

namespace Infrastructure.Services;

public class SongService : ISongService
{
    private readonly IStorageService _storageService;
    private readonly IAudioMetadataReaderService _audioMetadataReaderService;
    private readonly ILoggerService _logger;
    private readonly IUnitOfWork _uow;

    public SongService(IStorageService storageService, IAudioMetadataReaderService audioMetadataReaderService, ILoggerService logger, IUnitOfWork uow)
    {
        _storageService = storageService;
        _audioMetadataReaderService = audioMetadataReaderService;
        _logger = logger;
        _uow = uow;
    }

    public async Task<Result> ReplaceAudioFileAsync(Song song, GlobalVariables.AudioSource audioSource, Stream audioStream)
    {
        if (audioStream is not { CanSeek: true, CanRead: true })
        {
            _logger.Log("Bad file stream", LogLevel.Error, new { audioStream.CanSeek, audioStream.CanRead });
            return Error.SomethingWrong;
        }
        
        audioStream.Position = 0;
        
        var (fileGuid, filePath) = await _storageService.UploadFileAsync(audioStream, StorageFolder.Audio);

        var audioSize = (int)audioStream.GetKilobytes();
        var audioLength = _audioMetadataReaderService.GetMp3Duration(audioStream);
        
        song.UpdateAudioInfo(audioSize, audioSource, audioLength, fileGuid);
        
        _uow.SongRepository.Update(song);
        await _uow.SaveChangesAsync();
        
        return Result.Success();
    }
}