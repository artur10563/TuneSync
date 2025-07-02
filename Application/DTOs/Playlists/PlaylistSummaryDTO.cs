using Application.Projections.Playlists;


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
        public static PlaylistSummaryDTO FromProjection(PlaylistSummaryProjection projection)
        {
            return new PlaylistSummaryDTO(
                projection.Guid,
                projection.Title,
                projection.ThumbnailUrl,
                projection.IsFavorite,
                projection.SongCount);
        }
    }
}