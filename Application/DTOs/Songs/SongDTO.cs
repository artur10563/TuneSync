using Application.DTOs.Artists;
using Domain.Entities;
using System;
using Domain.Helpers;
using static Domain.Primitives.GlobalVariables;

namespace Application.DTOs.Songs
{
    public record SongDTO(
        Guid Guid,
        string Title,
        DateTime CreatedAt,
        string Source, //Youtube, deezer, upload from PC, etc
        string SourceUrl, //Link to YT video, etc
        string ThumbnailUrl,
        string AudioPath,
        int AudioSize,
        TimeSpan AudioLength,
        ArtistInfoDTO Artist,
        Guid? AlbumGuid,
        string? Album,
        bool IsFavorite
    )
    {
        public static SongDTO Create(Song song, Guid userGuid)
        {
            return new SongDTO(
                song.Guid,
                song.Title,
                song.CreatedAt,
                song.Source,
                SourceUrl: song.Source == SongSource.YouTube ? YoutubeHelper.GetYoutubeChannel(song.SourceId!) : "",
                YoutubeHelper.GetYoutubeThumbnail(song.SourceId),
                GetFirebaseMP3Link(song.AudioPath),
                song.AudioSize,
                song.AudioLength,
                ArtistInfoDTO.Create(song.Artist),
                AlbumGuid: song.AlbumGuid,
                Album: song.Album?.Title,
                IsFavorite: userGuid != Guid.Empty && song.FavoredBy.Any(us => us.UserGuid == userGuid && us.IsFavorite)
            );
        }

        public static SongDTO Create(Song song, Artist artist, bool isFavorited) => 
            Create(song, song.Album, artist, isFavorited);
          
        
        
        public static SongDTO Create(Song song, Album? album, Artist artist, bool isFavorited)
        {
            return new SongDTO(
                song.Guid,
                song.Title,
                song.CreatedAt,
                song.Source,
                SourceUrl: song.Source == SongSource.YouTube ? YoutubeHelper.GetYoutubeChannel(song.SourceId!) : "",
                YoutubeHelper.GetYoutubeThumbnail(song.SourceId),
                GetFirebaseMP3Link(song.AudioPath),
                song.AudioSize,
                song.AudioLength,
                ArtistInfoDTO.Create(artist),
                AlbumGuid: song.AlbumGuid,
                Album: album?.Title,
                IsFavorite: isFavorited // Precomputed value
            );
        }

        public static List<SongDTO> Create(IEnumerable<Song> songs, Guid userGuid)
        {
            return songs.Select(s => Create(s, userGuid)).ToList();
        }
    }
}