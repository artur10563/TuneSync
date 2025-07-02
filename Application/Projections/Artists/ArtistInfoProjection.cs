namespace Application.Projections.Artists;

public sealed record ArtistInfoProjection(
    Guid Guid,
    string Name,
    string DisplayName,
    string YoutubeChannelId,
    string? ThumbnailUrl
);