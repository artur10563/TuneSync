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


    public DownloadPlaylistFromYoutubeJob(IStorageService storageService, IUnitOfWork uow, IYoutubeService youtubeService)
    {
        _storageService = storageService;
        _uow = uow;
        _youtubeService = youtubeService;
    }


    public async Task<Guid> ExecuteAsync(string playlistId, Guid createdBy, CancellationToken cancellationToken)
    {
        Console.WriteLine("Starting the job");
        Console.WriteLine("Started fetching playlists from youtube");
        //Get all playlist songs
        var songs = await _youtubeService.GetPlaylistVideosAsync(playlistId);
        var newSourceIds = songs.Select(s => s.Id);

        Console.WriteLine("Finished fetching playlists from youtube");


        //Filter out existing songs, so duplicates are not downloaded
        var existingSongIds =
            _uow.SongRepository.Where(song => newSourceIds.Contains(song.SourceId), asNoTracking: true)
                .Select(song => new { song.Guid, song.SourceId });

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
        }

        //Get or create a playlist
        var playlist = await _uow.PlaylistRepository.FirstOrDefaultAsync(x => x.Source == PlaylistSource.YouTube && x.SourceId == playlistId,
            includes: pl => pl.Songs);

        if (playlist == null)
        {
            playlist = new Playlist(title: songs.First().Description, createdBy, PlaylistSource.YouTube, playlistId, artist.Guid);
            _uow.PlaylistRepository.Insert(playlist);
        }

        //Check if playlist has any songs of downloaded playlist
        var existingGuids = existingSongIds.Select(x => x.Guid).ToList();
        var playlistGuids = playlist.Songs.Select(song => song.Guid).ToList();
        var existingToInsert = existingGuids.Except(playlistGuids).ToList();

        //Add existing db songs to playlist
        var newPlaylistSongs = existingToInsert.Select(songGuid =>
            new PlaylistSong(playlist.Guid, songGuid)
        ).ToList();
        Console.WriteLine("Finished artists, playlists and existing songs");
        //Saved EVERY TIME a file is downloaded, since it can't be rolled back

        Console.WriteLine("Started processing files");

        var existingSongs = _uow.SongRepository.Where(song => newSourceIds.Contains(song.SourceId));
        foreach (var existingSong in existingSongs)
        {
            existingSong.Source = PlaylistSource.YouTube;
            _uow.SongRepository.Update(existingSong);
        }

        foreach (var song in songsToDownload)
        {
            Console.WriteLine($"Started {song.Title}");

            var (videoInfo, streamInfo) = await _youtubeService.GetVideoInfoAsync(GlobalVariables.GetYoutubeVideo(song.Id));
            await using var stream = await _youtubeService.GetAudioStreamAsync(streamInfo);
            Console.WriteLine($"Got info");
            var fileGuid = await _storageService.UploadFileAsync(stream);
            Console.WriteLine($"Sent file to firebase");

            var newSong = new Song(
                song.Title,
                SongSource.YouTube,
                song.Id,
                fileGuid,
                videoInfo.Duration.Value,
                (int)streamInfo.Size.KiloBytes,
                createdBy,
                artist.Guid
            );

            _uow.SongRepository.Insert(newSong);
            await _uow.SaveChangesAsync();
            Console.WriteLine($"Saved");

            newPlaylistSongs.Add(new PlaylistSong(playlist.Guid, newSong.Guid));
        }


        _uow.PlaylistSongRepository.InsertRange(newPlaylistSongs);
        await _uow.SaveChangesAsync();
        Console.WriteLine($"Saved whole playlist");

        Console.WriteLine("Finished the job");

        return playlist.Guid;
    }
}