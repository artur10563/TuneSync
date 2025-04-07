using Application.Repositories.Shared;
using Application.Services;
using Domain.Primitives;

namespace Application.BackgroundJobs;

public sealed class AudioFileCleanupJob
{
    public static string Id => "audio-file-cleanup-job";
    private readonly IUnitOfWork _uow;
    private readonly IStorageService _storageService;
    private readonly ILoggerService _logger;

    public AudioFileCleanupJob(IUnitOfWork uow, IStorageService storageService, ILoggerService logger)
    {
        _uow = uow;
        _storageService = storageService;
        _logger = logger;
    }


    /// <returns>Amount of deleted files</returns>
    public async Task<Result<int>> ExecuteAsync()
    {
        _logger.Log("Audio file cleanup started", LogLevel.Information);

        var dbFiles = _uow.SongRepository.Queryable().Select(x => x.AudioPath).ToHashSet();
        int counter = 0;
        
        await foreach (var file in _storageService.GetFileNames())
        {
            if (!file.EndsWith(".mp3")) continue;
            var fileName = file.Replace(".mp3", "");

            if (!Guid.TryParse(fileName, out var fileGuid))
            {
                _logger.Log("File failed", LogLevel.Warning, fileName);
                continue;
            }

            if (dbFiles.Contains(fileGuid)) continue;

            if (await _storageService.TryDeleteFileAsync(file))
            {
                _logger.Log("Deleted file", LogLevel.Information, fileName);
                ++counter;
            }
        }
        _logger.Log("Audio file cleanup completed", LogLevel.Information, counter);
        return counter;
    }
}