namespace Application.Projections.Playlists;

public record PlaylistSummaryProjection(
    Guid Guid,
    string Title,
    string ThumbnailUrl,
    bool IsFavorite,
    int SongCount
);