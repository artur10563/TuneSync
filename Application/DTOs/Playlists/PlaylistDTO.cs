using Application.DTOs.Songs;
using Domain.Entities;
using Domain.Helpers;
using Domain.Primitives;

namespace Application.DTOs.Playlists
{
    public record PlaylistDTO(
        Guid Guid,
        string Title,
        Guid CreatedBy,
        string CreatedByName,
        DateTime CreatedAt,
        DateTime ModifiedAt,
        string ThumbnailUrl,
        bool IsFavorite,
        int SongCount,
        PaginatedResponse<ICollection<SongDTO>> Songs
    )
    {
        public static PlaylistDTO Create(Playlist playlist, Guid userGuid, PageInfo pageInfo, int songCount)
        {
            return new PlaylistDTO(
                playlist.Guid,
                playlist.Title,
                //Will be replaced by UserDto
                playlist.CreatedBy,
                CreatedByName: playlist?.User.Name ?? string.Empty,
                playlist.CreatedAt,
                playlist.ModifiedAt,
                ThumbnailUrl: string.Empty,
                Songs: new PaginatedResponse<ICollection<SongDTO>>(SongDTO.Create(playlist.Songs, userGuid), pageInfo),
                IsFavorite: false,
                SongCount: songCount
            );
        }
        
        public static PlaylistDTO Create(Album album, List<SongDTO> songs,  PageInfo pageInfo, bool isFavorite, int songCount)
        {
            return new PlaylistDTO(
                album.Guid,
                album.Title,
                //Will be replaced by UserDto
                album.CreatedBy,
                CreatedByName: album?.User.Name ?? string.Empty,
                album.CreatedAt,
                album.ModifiedAt,
                ThumbnailUrl: string.Empty,
                Songs: new PaginatedResponse<ICollection<SongDTO>>(songs, pageInfo),
                IsFavorite: isFavorite,
                SongCount: songCount
            );
        }

        public static PlaylistDTO Create(Playlist playlist, List<SongDTO> songs,  PageInfo pageInfo, bool isFavorite, int songCount)
        {
            return new PlaylistDTO(
                playlist.Guid,
                playlist.Title,
                //Will be replaced by UserDto
                playlist.CreatedBy,
                CreatedByName: playlist?.User.Name ?? string.Empty,
                playlist.CreatedAt,
                playlist.ModifiedAt,
                ThumbnailUrl: string.Empty,
                Songs: new PaginatedResponse<ICollection<SongDTO>>(songs, pageInfo),
                IsFavorite: isFavorite, 
                SongCount: songCount
            );
        }
    }
}