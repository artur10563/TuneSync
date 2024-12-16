using Application.DTOs.Songs;
using Domain.Entities;

namespace Application.DTOs.Playlists
{
    public record PlaylistDTO(
        Guid Guid,
        string Title,
        Guid CreatedBy,
        string CreatedByName,
        DateTime CreatedAt,
        DateTime ModifiedAt,
        ICollection<SongDTO> Songs
    )
    {
        public static PlaylistDTO Create(Playlist playlist, Guid userGuid)
        {
            return new PlaylistDTO(
                playlist.Guid,
                playlist.Title,
                //Will be replaced by UserDto
                playlist.CreatedBy,
                CreatedByName: playlist?.User.Name ?? string.Empty,
                playlist.CreatedAt,
                playlist.ModifiedAt,
                Songs: SongDTO.Create(playlist.Songs,userGuid)
            );
        }
        
        public static PlaylistDTO Create(Playlist playlist, List<SongDTO> songs)
        {
            return new PlaylistDTO(
                playlist.Guid,
                playlist.Title,
                //Will be replaced by UserDto
                playlist.CreatedBy,
                CreatedByName: playlist?.User.Name ?? string.Empty,
                playlist.CreatedAt,
                playlist.ModifiedAt,
                Songs: songs
            );
        }
    }
}