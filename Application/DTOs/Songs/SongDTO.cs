﻿using Application.DTOs.Artists;
using Domain.Entities;
using System;
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
            var songAlbum = song.Playlists
                .FirstOrDefault(p => p.Source == PlaylistSource.YouTube);

            return new SongDTO(
                song.Guid,
                song.Title,
                song.CreatedAt,
                song.Source,
                SourceUrl: song.Source == SongSource.YouTube ? GetYoutubeChannel(song.SourceId!) : "",
                GetYoutubeThumbnail(song.SourceId),
                GetFirebaseMP3Link(song.AudioPath),
                song.AudioSize,
                song.AudioLength,
                ArtistInfoDTO.Create(song.Artist),
                AlbumGuid: songAlbum?.Guid,
                Album: songAlbum?.Title,
                IsFavorite: userGuid != Guid.Empty && song.FavoredBy.Any(u => u.Guid == userGuid)
            );
        }
        
        public static SongDTO Create(Song song, bool isFavorited)
        {
            var songAlbum = song.Playlists
                .FirstOrDefault(p => p.Source == PlaylistSource.YouTube);

            return new SongDTO(
                song.Guid,
                song.Title,
                song.CreatedAt,
                song.Source,
                SourceUrl: song.Source == SongSource.YouTube ? GetYoutubeChannel(song.SourceId!) : "",
                GetYoutubeThumbnail(song.SourceId),
                GetFirebaseMP3Link(song.AudioPath),
                song.AudioSize,
                song.AudioLength,
                ArtistInfoDTO.Create(song.Artist),
                AlbumGuid: songAlbum?.Guid,
                Album: songAlbum?.Title,
                IsFavorite: isFavorited // Precomputed value
            );
        }
        
        public static List<SongDTO> Create(IEnumerable<Song> songs, Guid userGuid)
        {
            return songs.Select(s => Create(s, userGuid)).ToList();
        }
    }
}