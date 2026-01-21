using Application.BackgroundJobs;
using Application.Repositories.Shared;
using Application.Services;
using Domain.Enums;
using Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.BackgroundJobs;

public sealed class FileCleanupJob : IFileCleanupJob
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
    public async Task<Result<int>> ExecuteAsync(Unit input, CancellationToken cancellationToken)
    {
        const string imgExtension = ".jpg";
        const string audioExtension = ".mp3";

        var counter = 0;

        try
        {
            _logger.Log("Audio file cleanup started", LogLevel.Information);

            var filePaths = (await _uow.SongRepository
                    .NoTrackingQueryable()
                    .Where(x => x.AudioPath != null)
                    .Select(x => $"{StorageFolder.Audio.GetPath()}/{x.AudioPath!.Value.ToString()}")
                    .ToHashSetAsync(cancellationToken))
                .Union(
                    await _uow.AlbumRepository
                        .NoTrackingQueryable()
                        .Where(x => x.ThumbnailSource == GlobalVariables.PlaylistSource.YouTubeMusic && x.ThumbnailId != null)
                        .Select(x => x.ThumbnailId)
                        .ToHashSetAsync(cancellationToken)
                ).ToHashSet();
            
            await foreach (var file in _storageService.GetFileNames().WithCancellation(cancellationToken))
            {
                if (!(file.EndsWith(imgExtension) || file.EndsWith(audioExtension))) continue;

                var fileName = file.Replace(imgExtension, "").Replace(audioExtension, "");

                if (filePaths.Contains(fileName)) continue;

                if (!await _storageService.TryDeleteFileAsync(file)) continue;

                _logger.Log("Deleted file", LogLevel.Information, fileName);
                ++counter;
            }

            _logger.Log("File cleanup completed", LogLevel.Information, counter);
        }
        catch (Exception ex)
        {
            _logger.Log("File cleanup failed", LogLevel.Error, ex);
        }

        return counter;
    }
}