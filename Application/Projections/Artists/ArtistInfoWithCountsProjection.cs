namespace Application.Projections.Artists;

public record ArtistInfoWithCountsProjection(
    Guid Guid,
    string Name,
    string DisplayName,
    string ChannelId,
    string? ThumbnailUrl,
    string? ParentName,
    int SongCount,
    int AlbumCount,
    int ChildrenCount);