using Domain.Entities;
using Domain.Primitives;

namespace Application.DTOs.Playlists
{
    public record PlaylistSummaryDTO(
        Guid Guid,
        string Title,
        string ThumbnailUrl,
        bool IsFavorite
    )
    {
        public static PlaylistSummaryDTO Create(Playlist playlist, bool isFavorite)
        {
            return new PlaylistSummaryDTO(
                playlist.Guid,
                playlist.Title,
                ThumbnailUrl: playlist.ThumbnailSource == GlobalVariables.PlaylistSource.YouTube
                    ? GlobalVariables.GetYoutubePlaylistThumbnail(playlist.ThumbnailId)
                    : "",
                IsFavorite: isFavorite);
        }
        
        public static PlaylistSummaryDTO Create(Playlist playlist, Guid userGuid)
        {
            return new PlaylistSummaryDTO(
                playlist.Guid,
                playlist.Title,
                ThumbnailUrl: playlist.ThumbnailSource == GlobalVariables.PlaylistSource.YouTube
                    ? GlobalVariables.GetYoutubePlaylistThumbnail(playlist.ThumbnailId)
                    : "",
                IsFavorite: playlist.FavoredBy.Any(ufp => ufp.UserGuid == userGuid && ufp.PlaylistGuid == playlist.Guid && ufp.IsFavorite)
                );
        }
        
        public static PlaylistSummaryDTO Create(Album album, bool isFavorite)
        {
            return new PlaylistSummaryDTO(
                album.Guid,
                album.Title,
                ThumbnailUrl: album.ThumbnailSource == GlobalVariables.PlaylistSource.YouTube
                    ? GlobalVariables.GetYoutubePlaylistThumbnail(album.ThumbnailId)
                    : "",
                IsFavorite: isFavorite);
        }
        
        
        
        public static List<PlaylistSummaryDTO> Create(ICollection<Playlist> playlists)
        {
            return playlists.Select(p=>Create(p,false)).ToList(); //INCORRECT
        }
        
        public static List<PlaylistSummaryDTO> Create(ICollection<Album> album, Guid userGuid)
        {
            return album.Select(a=>
            {
                
                var v = a.FavoredBy.ToList();
                var isFav = a.FavoredBy.Any(x => x.UserGuid == userGuid && x.IsFavorite && x.AlbumGuid == a.Guid);
                return Create(a, isFav);
            }).ToList(); //INCORRECT
        }
    }
}