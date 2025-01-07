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

//TODO: TEST, ADD NORMAL LOGGER
    public DownloadPlaylistFromYoutubeJob(IStorageService storageService, IUnitOfWork uow, IYoutubeService youtubeService)
    {
        _storageService = storageService;
        _uow = uow;
        _youtubeService = youtubeService;
    }


    public async Task<Guid> ExecuteAsync(string youtubePlaylistId, Guid createdBy, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("Starting the job");
            Console.WriteLine("Started fetching playlists from youtube");
            //Get all playlist songs
            var (songs, playlistThumbnailId) = await _youtubeService.GetPlaylistVideosAsync(youtubePlaylistId);
            var newSourceIds = songs.Select(s => s.Id);

            Console.WriteLine("Finished fetching playlists from youtube");

            //check if song table has songs with  SOURCE ID + song ALBUM GUID

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
            }

            
            Console.WriteLine("Finished artists and albums");
            //Saved EVERY TIME a file is downloaded, since it can't be rolled back

            Console.WriteLine("Started processing files");

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
                    artist.Guid,
                    album.Guid
                );

                _uow.SongRepository.Insert(newSong);
                await _uow.SaveChangesAsync();
                Console.WriteLine($"Saved");

            }

            await _uow.SaveChangesAsync();
            
            Console.WriteLine($"Saved whole album");
            Console.WriteLine("Finished the job");

            return album.Guid;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return Guid.Empty;
    }
}