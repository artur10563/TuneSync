using Application.DTOs.Songs;
using Domain.Entities;
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
                ThumbnailUrl: playlist.ThumbnailSource == GlobalVariables.PlaylistSource.YouTube
                    ? GlobalVariables.GetYoutubePlaylistThumbnail(playlist.ThumbnailId)
                    : "",
                Songs: SongDTO.Create(playlist.Songs, userGuid)
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
                ThumbnailUrl: playlist.ThumbnailSource == GlobalVariables.PlaylistSource.YouTube
                    ? GlobalVariables.GetYoutubePlaylistThumbnail(playlist.ThumbnailId)
                    : "",
                Songs: songs
            );
        }
    }
}