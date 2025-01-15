using Application.Repositories.Shared;
using Application.Services;
using Domain.Entities;
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


    public async Task<Guid> ExecuteAsync(string youtubePlaylistId, Guid createdBy, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Log("Fetching playlist from YouTube", LogLevel.Information);

            //Get all playlist songs
            var (songs, playlistThumbnailId) = await _youtubeService.GetPlaylistVideosAsync(youtubePlaylistId);
            var newSourceIds = songs.Select(s => s.Id);

            _logger.Log("Fetching playlist from YouTube", LogLevel.Information);


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
                artist = new Artist(name: ytAuthor.Title, youtubeChannelId: ytAuthor.Id);
                _uow.ArtistRepository.Insert(artist);
                _logger.Log($"Created new artist", LogLevel.Information, new { artist.Guid, artist.Name });
            }

            //Get or create album
            var album = await _uow.AlbumRepository.FirstOrDefaultAsync(x => x.SourceId == youtubePlaylistId,
                includes: pl => pl.Songs);

            if (album == null)
            {
                album = new Album(
                    title: songs.First().Description,
                    createdBy: createdBy,
                    sourceId: youtubePlaylistId,
                    artistGuid: artist.Guid,
                    thumbnailSource: PlaylistSource.YouTube,
                    thumbnailId: playlistThumbnailId);
                _uow.AlbumRepository.Insert(album);
                _logger.Log($"Created new album", LogLevel.Information, new { album.Guid, album.Title });
            }


            _logger.Log("Artist and album setup completed", LogLevel.Information);

            //Saved EVERY TIME a file is downloaded, since it can't be rolled back
            _logger.Log("Started processing files", LogLevel.Information);

            var existingSongs = _uow.SongRepository.Where(song => newSourceIds.Contains(song.SourceId));
            foreach (var existingSong in existingSongs)
            {
                existingSong.Source = PlaylistSource.YouTube;
                existingSong.AlbumGuid = album.Guid;
            }

            if (existingSongs.Any())
                _uow.SongRepository.UpdateRange(existingSongs);

            foreach (var song in songsToDownload)
            {
                try
                {



                    _logger.Log($"Started {song.Title}", LogLevel.Information, new { song.Id, song.Title });

                    var (videoInfo, streamInfo) = await _youtubeService.GetVideoInfoAsync(GlobalVariables.GetYoutubeVideo(song.Id));
                    await using var stream = await _youtubeService.GetAudioStreamAsync(streamInfo);

                    _logger.Log($"Video info retrieved", LogLevel.Information);

                    var fileGuid = await _storageService.UploadFileAsync(stream);

                    _logger.Log($"File uploaded to Firebase", LogLevel.Information, fileGuid);

                    var newSong = new Song(
                        song.Title,
                        SongSource.YouTube,
                        song.Id,
                        fileGuid,
                        videoInfo.Duration.Value,
                        (int)streamInfo.Size.KiloBytes,
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
        }

        return Guid.Empty;
    }
}