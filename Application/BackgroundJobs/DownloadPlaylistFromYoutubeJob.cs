using Application.DTOs.Songs;
using Application.DTOs.Youtube;
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

internal sealed class YoutubeAlbumCreationContext
{
    public string PlaylistId { get; init; }
    public IReadOnlyList<YoutubeSongInfo> Songs { get; init; }
    public SongThumbnail Thumbnail { get; init; }
    public bool IsYoutubeMusic { get; init; }
    public Guid CreatedBy { get; init; }
    public Guid ArtistGuid { get; init; }
    public string Source { get; init; }
}

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

            if (songs.Count > AlbumConstants.MaxYoutubeAlbumLength)
            {
                _logger.Log("Attempt to download bad album", LogLevel.Warning, new { youtubePlaylistId, count = songs.Count });
                return YoutubeError.MaxYoutubeLengthError;
            }

            //Filter out existing songs, so duplicates are not downloaded
            var (songsToDownload, existingSongs) = GetSongsToDownload(songs);

            //Get or create an artist
            var artist = await CreateOrGetArtistAsync(songs.First().Author);

            //Get or create album
            var album = await CreateOrGetAlbumAsync(new YoutubeAlbumCreationContext
            {
                ArtistGuid = artist.Guid,
                CreatedBy = createdBy,
                IsYoutubeMusic = isYTM,
                Source = source,
                PlaylistId = youtubePlaylistId,
                Songs = songs,
                Thumbnail = playlistThumbnail
            }, cancellationToken);


            _logger.Log("Artist and album setup completed", LogLevel.Information);

            //Saved EVERY TIME a file is downloaded, since it can't be rolled back
            _logger.Log("Started processing files", LogLevel.Information);

            foreach (var existingSong in existingSongs)
            {
                existingSong.Source = source;
                existingSong.AlbumGuid = album.Guid;
            }

            if (existingSongs.Count != 0)
                _uow.SongRepository.UpdateRange(existingSongs);

            foreach (var song in songsToDownload)
            {
                var newSong = await CreateNewSongAsync(createdBy, song, artist, album);

                _uow.SongRepository.Insert(newSong);
                await _uow.SaveChangesAsync();

                _logger.Log($"Saved song {newSong.Title}", LogLevel.Information);
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

    #region Private

    private async Task<Song> CreateNewSongAsync(Guid createdBy, YoutubeSongInfo song, Artist artist, Album album)
    {
        Song newSong;
        // Try to get media info. If failed - still save, but without media info.
        try
        {
            _logger.Log($"Started {song.Title}", LogLevel.Information, new { song.Id, song.Title });

            var videoInfo = await _youtubeService.GetVideoInfoAsyncDLP(song.Id);

            _logger.Log($"Video info retrieved", LogLevel.Information);

            _logger.Log($"Trying to get audio stream", LogLevel.Information);

            await using var stream = await _youtubeService.GetAudioStreamAsyncDLP(song.Id);

            _logger.Log($"Audio stream retrieved", LogLevel.Information);

            var (filePathGuid, _) = await _storageService.UploadFileAsync(stream, StorageFolder.Audio);

            _logger.Log($"File uploaded to Firebase", LogLevel.Information, filePathGuid);

            newSong = Song.CreateWithAudio(
                song.Title,
                SongSource.YouTube,
                song.Id,
                filePathGuid,
                videoInfo.Duration,
                (int)stream.GetKilobytes(),
                createdBy,
                artist.Guid,
                album.Guid
            );
            _logger.Log($"Creating new song with audio", LogLevel.Information);
        }
        catch (Exception e)
        {
            newSong = Song.CreateWithoutAudio(
                song.Title,
                SongSource.YouTube,
                song.Id,
                createdBy,
                artist.Guid,
                album.Guid
            );
            _logger.Log($"Failed to get audioStream. Creating new song without audio", LogLevel.Information, context: e.Message);
        }

        return newSong;
    }


    private (List<YoutubeSongInfo> toDownload, List<Song> existingSongs) GetSongsToDownload(List<YoutubeSongInfo> songs)
    {
        var newSourceIds = songs.Select(s => s.Id);

        var existingSongs = _uow.SongRepository
            .Where(song => newSourceIds.Contains(song.SourceId))
            .ToList();

        var existingSourceIds = existingSongs.Select(es => es.SourceId).ToHashSet();
        var songsToDownload = songs
            .Where(song => !existingSourceIds.Contains(song.Id))
            .ToList();

        return (songsToDownload, existingSongs);
    }

    private async Task<Artist> CreateOrGetArtistAsync(SongAuthor ytAuthor)
    {
        var artist = await _uow.ArtistRepository.FirstOrDefaultAsync(x => x.YoutubeChannelId == ytAuthor.Id);
        if (artist != null) return artist;

        // Create artist
        var artistInfo = await _youtubeService.GetChannelInfoAsync(ytAuthor.Id);
        artist = new Artist(
            name: ytAuthor.Title,
            youtubeChannelId: ytAuthor.Id,
            thumbnailUrl: artistInfo?.Thumbnail?.Url);
        _uow.ArtistRepository.Insert(artist);
        _logger.Log($"Created new artist", LogLevel.Information, artistInfo);

        return artist;
    }

    private async Task<Album> CreateOrGetAlbumAsync(YoutubeAlbumCreationContext ctx, CancellationToken cancellationToken)
    {
        var album = await _uow.AlbumRepository.FirstOrDefaultAsync(x => x.SourceId == ctx.PlaylistId,
            includes: pl => pl.Songs);

        if (album != null) return album;

        // Create album
        var playlistThumbnailId = ctx.Thumbnail.Url;

        if (ctx.IsYoutubeMusic)
        {
            var httpClient = new HttpClient();
            await using var stream = await httpClient.GetStreamFromUrlAsync(ctx.Thumbnail.Url, cancellationToken);
            (_, playlistThumbnailId) = await _storageService.UploadFileAsync(stream, StorageFolder.Images);
        }

        album = new Album(
            title: ctx.Songs[0].Description,
            createdBy: ctx.CreatedBy,
            sourceId: ctx.PlaylistId,
            artistGuid: ctx.ArtistGuid,
            thumbnailSource: ctx.Source,
            thumbnailId: playlistThumbnailId);
        album.ExpectedSongs = ctx.Songs.Count;
        _uow.AlbumRepository.Insert(album);
        _logger.Log($"Created new album", LogLevel.Information, new { album.Guid, album.Title });

        return album;
    }

    #endregion
}