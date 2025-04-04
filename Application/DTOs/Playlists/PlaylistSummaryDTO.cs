using Application.DTOs.Artists;
using Domain.Entities;
using Domain.Helpers;
using Domain.Primitives;

namespace Application.DTOs.Playlists
{
    public record PlaylistSummaryDTO(
        Guid Guid,
        string Title,
        string ThumbnailUrl,
        bool IsFavorite,
        int SongCount
    )
    {
        public static PlaylistSummaryDTO Create(Playlist playlist, bool isFavorite, int songCount)
        {
            return new PlaylistSummaryDTO(
                playlist.Guid,
                playlist.Title,
                ThumbnailUrl: string.Empty,
                IsFavorite: isFavorite,
                SongCount: songCount);
        }

        public static PlaylistSummaryDTO Create(Playlist playlist, Guid userGuid, int songCount)
        {
            return new PlaylistSummaryDTO(
                playlist.Guid,
                playlist.Title,
                ThumbnailUrl: string.Empty,
                IsFavorite: playlist.FavoredBy.Any(ufp => ufp.UserGuid == userGuid && ufp.PlaylistGuid == playlist.Guid && ufp.IsFavorite),
                SongCount: songCount
            );
        }
    }
}