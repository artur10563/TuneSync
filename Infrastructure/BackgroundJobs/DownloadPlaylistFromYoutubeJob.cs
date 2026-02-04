using Application.BackgroundJobs;
using Application.DTOs.Songs;
using Application.Extensions;
using Application.Repositories.Shared;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Domain.Helpers;
using Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using static Domain.Primitives.GlobalVariables;

namespace Infrastructure.BackgroundJobs;

public sealed class DownloadPlaylistFromYoutubeJob : IDownloadPlaylistFromYoutubeJob
{
    private readonly IUnitOfWork _uow;
    private readonly IStorageService _storageService;
    private readonly IYoutubeService _youtubeService;
    private readonly ILoggerService _logger;
    private readonly ISongService _songService;

    private List<YoutubeSongInfo> _songs;
    private SongThumbnail _playlistThumbnail;
    private Artist _artist;
    private Album _album;
    private string _source;

    public DownloadPlaylistFromYoutubeJob(
        IStorageService storageService,
        IUnitOfWork uow,
        IYoutubeService youtubeService,
        ILoggerService logger,
        ISongService songService)
    {
        _storageService = storageService;
        _uow = uow;
        _youtubeService = youtubeService;
        _logger = logger;
        _songService = songService;
    }

    public async Task<Result<Guid>> ExecuteAsync(DownloadPlaylistFromYoutubeJobInput input, CancellationToken cancellationToken)
    {
        try
        {
            _source = YoutubeHelper.IsYoutubeMusic(input.YoutubePlaylistId)
                ? PlaylistSource.YouTubeMusic
                : PlaylistSource.YouTube;

            (_songs, _playlistThumbnail) = await _youtubeService.GetPlaylistVideosAsync(input.YoutubePlaylistId);
            _logger.Log($"Fetched playlist '{input.YoutubePlaylistId}' with {_songs.Count} songs from {_source}", LogLevel.Information);

            if (_songs.Count > AlbumConstants.MaxYoutubeAlbumLength)
            {
                _logger.Log("Playlist exceeds maximum length", LogLevel.Warning, new { input.YoutubePlaylistId, count = _songs.Count });
                return YoutubeError.MaxYoutubeLengthError;
            }

            // Plan import
            var (songsToDownload, songsToUpdate, existingSongs) = await PlanSongImportAsync();
            _logger.Log($"Import plan: {songsToDownload.Count} new, {songsToUpdate.Count} to update, {existingSongs.Count} existing", LogLevel.Information);

            // Get or create album and artist
            _artist = await GetOrCreateArtistAsync(_songs.First().Author);
            _album = await GetOrCreateAlbumAsync(input.CreatedBy, input.YoutubePlaylistId, cancellationToken);
            await _uow.SaveChangesAsync();

            
            // Bulk update existing songs
            await BulkUpdateExistingSongs(existingSongs);

            // Create new songs in batch, then save once
            var newSongs = await CreateNewSongsAsync(songsToDownload, input.CreatedBy);
            if (newSongs.Any())
            {
                foreach (var song in newSongs)
                    _uow.SongRepository.Insert(song);

                await _uow.SaveChangesAsync();
                _logger.Log($"Saved {newSongs.Count} new songs for album '{_album.Title}' ({_album.Guid})", LogLevel.Information);
            }

            // Update existing songs missing audio
            await ProcessSongsToUpdate(songsToUpdate);

            _logger.Log($"Finished processing album '{_album.Title}' ({_album.Guid})", LogLevel.Information);
            return _album.Guid;
        }
        catch (Exception e)
        {
            _logger.Log(e.Message, LogLevel.Error, exception: e);
            return Error.SomethingWrong;
        }
    }

    #region Private workflow steps

    private async Task<(List<YoutubeSongInfo> toDownload, List<Song> toUpdate, List<Song> existingSongs)> PlanSongImportAsync()
    {
        var newSourceIds = _songs.Select(s => s.Id).ToHashSet();

        var existingSongs = await _uow.SongRepository
            .IgnoreFilter(CommonFilter.HasAudioFilter)
            .Where(s => s.SourceId != null && newSourceIds.Contains(s.SourceId))
            .Select(s => new { Song = s, HasAudio = s.AudioPath.HasValue })
            .ToListAsync();

        var existingIds = existingSongs.Select(x => x.Song.SourceId).ToHashSet();

        var songsToDownload = _songs.Where(s => !existingIds.Contains(s.Id)).ToList();
        var songsToUpdate = existingSongs.Where(x => !x.HasAudio).Select(x => x.Song).ToList();

        return (songsToDownload, songsToUpdate, existingSongs.Select(x => x.Song).ToList());
    }

    private async Task<Artist> GetOrCreateArtistAsync(SongAuthor ytAuthor)
    {
        var artist = await _uow.ArtistRepository.FirstOrDefaultAsync(x => x.YoutubeChannelId == ytAuthor.Id);
        if (artist != null) return artist;

        var artistInfo = await _youtubeService.GetChannelInfoAsync(ytAuthor.Id);
        artist = new Artist(
            name: ytAuthor.Title,
            youtubeChannelId: ytAuthor.Id,
            thumbnailUrl: artistInfo.Thumbnail?.Url);

        _uow.ArtistRepository.Insert(artist);
        _logger.Log("Created new artist", LogLevel.Information, artistInfo);

        return artist;
    }

    private async Task<Album> GetOrCreateAlbumAsync(Guid createdBy, string playlistId, CancellationToken cancellationToken)
    {
        var album = await _uow.AlbumRepository.FirstOrDefaultAsync(
            x => x.SourceId == playlistId,
            includes: pl => pl.Songs);

        if (album != null) return album;

        var thumbnailId = _playlistThumbnail.Url;
        if (_source == PlaylistSource.YouTubeMusic)
        {
            await using var stream = await new HttpClient().GetStreamFromUrlAsync(_playlistThumbnail.Url, cancellationToken);
            (_, thumbnailId) = await _storageService.UploadFileAsync(stream, StorageFolder.Images);
        }

        album = new Album(
            title: _songs[0].Description,
            createdBy: createdBy,
            sourceId: playlistId,
            artistGuid: _artist.Guid,
            thumbnailSource: _source,
            thumbnailId: thumbnailId)
        {
            ExpectedSongs = _songs.Count
        };

        _uow.AlbumRepository.Insert(album);
        _logger.Log("Created new album", LogLevel.Information, new { album.Guid, album.Title });

        return album;
    }

    private async Task BulkUpdateExistingSongs(List<Song> existingSongs)
    {
        if (existingSongs.Count == 0) return;

        var guids = existingSongs.Select(s => s.Guid).ToHashSet();

        await _uow.SongRepository.BulkUpdatePropertyAsync(
            s => guids.Contains(s.Guid),
            (x => x.Source, _ => _source),
            (x => x.AlbumGuid!, _ => _album.Guid));

        _logger.Log($"Updated {existingSongs.Count} existing songs in bulk", LogLevel.Information);
    }

    private async Task<List<Song>> CreateNewSongsAsync(List<YoutubeSongInfo> songsToDownload, Guid createdBy)
    {
        var newSongs = new List<Song>();

        foreach (var songInfo in songsToDownload)
        {
            try
            {
                var videoInfo = await _youtubeService.GetVideoInfoAsyncDLP(songInfo.Id);
                await using var audioStream = await _youtubeService.GetAudioStreamAsyncDLP(songInfo.Id);
                var (filePathGuid, _) = await _storageService.UploadFileAsync(audioStream, StorageFolder.Audio);

                newSongs.Add(Song.CreateWithAudio(
                    songInfo.Title,
                    SongSource.YouTube,
                    songInfo.Id,
                    filePathGuid,
                    videoInfo.Duration,
                    (int)audioStream.GetKilobytes(),
                    createdBy,
                    _artist.Guid,
                    _album.Guid));
            }
            catch (YoutubeFetchException e)
            {
                _logger.Log($"Failed to fetch audio for '{songInfo.Title}', creating without audio", LogLevel.Warning, context: e.Message);
                newSongs.Add(Song.CreateWithoutAudio(
                    songInfo.Title,
                    SongSource.YouTube,
                    songInfo.Id,
                    createdBy,
                    _artist.Guid,
                    _album.Guid));
            }
        }

        return newSongs;
    }

    private async Task ProcessSongsToUpdate(List<Song> songsToUpdate)
    {
        foreach (var song in songsToUpdate)
        {
            if (song.SourceId == null)
            {
                _logger.Log("Skipping song without SourceId", LogLevel.Warning, new { song.Title });
                continue;
            }


            try
            {
                await using var stream = await _youtubeService.GetAudioStreamAsyncDLP(song.SourceId);
                await _songService.ReplaceAudioFileAsync(song, AudioSource.Youtube, stream);
                _logger.Log($"Updated song '{song.Title}' ({song.Guid})", LogLevel.Information);
            }
            catch (YoutubeFetchException e)
            {
                _logger.Log($"Failed to fetch audio for '{song.Title}'", LogLevel.Warning, context: e.Message);
            }
        }

        if (songsToUpdate.Count != 0)
            _logger.Log($"Updated {songsToUpdate.Count} songs with missing audio", LogLevel.Information);
    }

    #endregion
}