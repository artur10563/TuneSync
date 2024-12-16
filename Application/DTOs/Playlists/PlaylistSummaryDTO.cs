using Domain.Entities;

namespace Application.DTOs.Playlists
{
    public record PlaylistSummaryDTO(
        Guid Guid,
        string Title
    )
    {
        public static PlaylistSummaryDTO Create(Playlist playlist)
        {
            return new PlaylistSummaryDTO(playlist.Guid, playlist.Title);
        }
    }
}