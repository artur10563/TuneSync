using Application.DTOs.Artists;
using Domain.Entities;
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
                ThumbnailUrl: playlist.ThumbnailSource == GlobalVariables.PlaylistSource.YouTube
                    ? GlobalVariables.GetYoutubePlaylistThumbnail(playlist.ThumbnailId)
                    : "",
                IsFavorite: isFavorite,
                SongCount: songCount);
        }

        public static PlaylistSummaryDTO Create(Playlist playlist, Guid userGuid, int songCount)
        {
            return new PlaylistSummaryDTO(
                playlist.Guid,
                playlist.Title,
                ThumbnailUrl: playlist.ThumbnailSource == GlobalVariables.PlaylistSource.YouTube
                    ? GlobalVariables.GetYoutubePlaylistThumbnail(playlist.ThumbnailId)
                    : "",
                IsFavorite: playlist.FavoredBy.Any(ufp => ufp.UserGuid == userGuid && ufp.PlaylistGuid == playlist.Guid && ufp.IsFavorite),
                SongCount: songCount
            );
        }
        
        public static List<PlaylistSummaryDTO> Create(ICollection<Playlist> playlists)
        {
            return playlists.Select(p => Create(p, false, 0)).ToList();
        }
        
    }
}