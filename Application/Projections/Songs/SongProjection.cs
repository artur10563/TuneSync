using Application.Projections.Artists;

namespace Application.Projections.Songs;

public record SongProjection(
    Guid Guid,
    string Title,
    DateTime CreatedAt,
    string Source,
    string SourceId,
    Guid AudioPath,
    int AudioSize,
    TimeSpan AudioLength,
    ArtistInfoProjection Artist,
    Guid? AlbumGuid,
    string? Album,
    bool IsFavorite
);