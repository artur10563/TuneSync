using Domain.Entities;
using Domain.Primitives;

namespace Application.DTOs.Playlists
{
    public record PlaylistSummaryDTO(
        Guid Guid,
        string Title,
        string ThumbnailUrl
    )
    {
        public static PlaylistSummaryDTO Create(Playlist playlist)
        {
            return new PlaylistSummaryDTO(
                playlist.Guid,
                playlist.Title,
                ThumbnailUrl: playlist.ThumbnailSource == GlobalVariables.PlaylistSource.YouTube
                    ? GlobalVariables.GetYoutubePlaylistThumbnail(playlist.ThumbnailId)
                    : "");
        }
        
        public static PlaylistSummaryDTO Create(Album album)
        {
            return new PlaylistSummaryDTO(
                album.Guid,
                album.Title,
                ThumbnailUrl: album.ThumbnailSource == GlobalVariables.PlaylistSource.YouTube
                    ? GlobalVariables.GetYoutubePlaylistThumbnail(album.ThumbnailId)
                    : "");
        }
        public static List<PlaylistSummaryDTO> Create(ICollection<Playlist> playlists)
        {
            return playlists.Select(Create).ToList();
        }
        
        public static List<PlaylistSummaryDTO> Create(ICollection<Album> album)
        {
            return album.Select(Create).ToList();
        }
    }
}