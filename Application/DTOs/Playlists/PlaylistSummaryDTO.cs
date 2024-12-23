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
        public static List<PlaylistSummaryDTO> Create(ICollection<Playlist> playlists)
        {
            return playlists.Select(p => new PlaylistSummaryDTO(p.Guid, p.Title)).ToList();
        }
    }
}