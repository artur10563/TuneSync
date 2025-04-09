using Application.Extensions;
using Application.Repositories.Shared;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.Helpers;
using Domain.Primitives;
using static Domain.Primitives.GlobalVariables;

namespace Application.BackgroundJobs;

public sealed class DownloadPlaylistFromYoutubeJob
{
    private readonly IUnitOfWork _uow;
    private readonly IStorageService _storageService;
    private readonly IYoutubeService _youtubeService;
    private readonly ILoggerService _logger;

    public DownloadPlaylistFromYoutubeJob(IStorageService storageService, IUnitOfWork uow, IYoutubeService youtubeService, ILoggerService logger)
    {
        _storageService = storageService;
        _uow = uow;
        _youtubeService = youtubeService;
        _logger = logger;
    }


    public async Task<Result<Guid>> ExecuteAsync(string youtubePlaylistId, Guid createdBy, CancellationToken cancellationToken)
    {
        try
        {
            var isYTM = YoutubeHelper.IsYoutubeMusic(youtubePlaylistId);
            var source = isYTM
                ? GlobalVariables.PlaylistSource.YouTubeMusic 
                : GlobalVariables.PlaylistSource.YouTube;
            
            _logger.Log($"Fetching playlist from {source}", LogLevel.Information);

            //Get all playlist songs
            var (songs, playlistThumbnail) = await _youtubeService.GetPlaylistVideosAsync(youtubePlaylistId);
            var newSourceIds = songs.Select(s => s.Id);

            if (songs.Count > AlbumConstants.MaxYoutubeAlbumLength)
            {
                _logger.Log("Attempt to download bad album", LogLevel.Warning, new { youtubePlaylistId, count = songs.Count });
                return YoutubeError.MaxYoutubeLengthError;
            }

            //Filter out existing songs, so duplicates are not downloaded
            var existingSongIds =
                _uow.SongRepository.Where(song => newSourceIds.Contains(song.SourceId), asNoTracking: true)
                    .Select(song => new { song.Guid, song.SourceId, song.AlbumGuid });

            var existingSourceIds = existingSongIds.Select(es => es.SourceId).ToHashSet();
            var songsToDownload = songs
                .Where(song => !existingSourceIds.Contains(song.Id))
                .ToList();

            //Get or create an artist
            var ytAuthor = songs.First().Author;
            var artist = await _uow.ArtistRepository.FirstOrDefaultAsync(x => x.YoutubeChannelId == ytAuthor.Id);
            if (artist == null)
            {
                var artistInfo = await _youtubeService.GetChannelInfoAsync(ytAuthor.Id);
                artist = new Artist(
                    name: ytAuthor.Title, 
                    youtubeChannelId: ytAuthor.Id, 
                    thumbnailUrl: artistInfo?.Thumbnail?.Url);
                _uow.ArtistRepository.Insert(artist);
                _logger.Log($"Created new artist", LogLevel.Information, artistInfo);
            }

            //Get or create album
            var album = await _uow.AlbumRepository.FirstOrDefaultAsync(x => x.SourceId == youtubePlaylistId,
                includes: pl => pl.Songs);

            if (album == null)
            {
                var playlistThumbnailId = playlistThumbnail.Url;
                //GetStreamFromUrl
                if (isYTM)
                {
                    var httpClient = new HttpClient();
                    await using var stream = await httpClient.GetStreamFromUrlAsync(playlistThumbnail.Url, cancellationToken);
                    playlistThumbnailId = await _storageService.UploadFileAsync(stream, StorageFolder.Images);
                }
                
                album = new Album(
                    title: songs.First().Description,
                    createdBy: createdBy,
                    sourceId: youtubePlaylistId,
                    artistGuid: artist.Guid,
                    thumbnailSource: source,
                    thumbnailId: playlistThumbnailId);
                album.ExpectedSongs = songs.Count;
                _uow.AlbumRepository.Insert(album);
                _logger.Log($"Created new album", LogLevel.Information, new { album.Guid, album.Title });
            }


            _logger.Log("Artist and album setup completed", LogLevel.Information);

            //Saved EVERY TIME a file is downloaded, since it can't be rolled back
            _logger.Log("Started processing files", LogLevel.Information);

            var existingSongs = _uow.SongRepository.Where(song => newSourceIds.Contains(song.SourceId));
            foreach (var existingSong in existingSongs)
            {
                existingSong.Source = source;
                existingSong.AlbumGuid = album.Guid;
            }

            if (existingSongs.Any())
                _uow.SongRepository.UpdateRange(existingSongs);

            foreach (var song in songsToDownload)
            {
                try
                {
                    _logger.Log($"Started {song.Title}", LogLevel.Information, new { song.Id, song.Title });
                    
                    var videoInfo = await _youtubeService.GetVideoInfoAsyncDLP(song.Id);
                    await using var stream = await _youtubeService.GetAudioStreamAsyncDLP(song.Id);

                    _logger.Log($"Video info retrieved", LogLevel.Information);

                    var fileGuid = await _storageService.UploadFileAsync(stream, StorageFolder.Audio);

                    _logger.Log($"File uploaded to Firebase", LogLevel.Information, fileGuid);

                    var newSong = new Song(
                        song.Title,
                        SongSource.YouTube,
                        song.Id,
                        new Guid(fileGuid),
                        videoInfo.Duration,
                        (int)stream.GetKilobytes(),
                        createdBy,
                        artist.Guid,
                        album.Guid
                    );

                    _uow.SongRepository.Insert(newSong);
                    await _uow.SaveChangesAsync();

                    _logger.Log($"Saved song", LogLevel.Information);
                }
                catch (Exception ex)
                {
                    _logger.Log(ex.Message, LogLevel.Error, ex);
                }
            }

            await _uow.SaveChangesAsync();

            _logger.Log($"Album saved", LogLevel.Information, new { album.Guid, album.Title });

            return album.Guid;
        }
        catch (Exception e)
        {
            _logger.Log(e.Message, LogLevel.Error, exception: e);
            return Error.SomethingWrong;
        }
    }
}