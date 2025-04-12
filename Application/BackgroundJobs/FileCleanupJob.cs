using Application.Repositories.Shared;
using Application.Services;
using Domain.Primitives;

namespace Application.BackgroundJobs;

public sealed class FileCleanupJob
{
    public static string Id => "file-cleanup-job";
    private readonly IUnitOfWork _uow;
    private readonly IStorageService _storageService;
    private readonly ILoggerService _logger;

    public FileCleanupJob(IUnitOfWork uow, IStorageService storageService, ILoggerService logger)
    {
        _uow = uow;
        _storageService = storageService;
        _logger = logger;
    }


    /// <returns>Amount of deleted files</returns>
    public async Task<Result<int>> ExecuteAsync()
    {
        _logger.Log("Audio file cleanup started", LogLevel.Information);

        var dbFiles = _uow.SongRepository
            .NoTrackingQueryable()
            .Select(x => x.GetAudioPath())
            .ToHashSet()
            .Union(
                _uow.AlbumRepository
                    .Where(x => x.ThumbnailSource == GlobalVariables.PlaylistSource.YouTubeMusic && x.ThumbnailId != null, asNoTracking: true)
                    .Select(x => x.ThumbnailId)
                    .ToHashSet()
            );

        var imgExtension = ".jpg";
        var audioExtension = ".mp3";
        
        int counter = 0;

        await foreach (var file in _storageService.GetFileNames())
        {
            if (! (file.EndsWith(imgExtension) || file.EndsWith(audioExtension)) ) continue;
            
            var fileName = file.Replace(imgExtension, "").Replace(audioExtension, "");

            if (dbFiles.Contains(fileName)) continue;

            if (await _storageService.TryDeleteFileAsync(file))
            {
                _logger.Log("Deleted file", LogLevel.Information, fileName);
                ++counter;
            }
        }
        _logger.Log("File cleanup completed", LogLevel.Information, counter);
        return counter;
    }
}