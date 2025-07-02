using Application.Projections.Artists;

namespace Application.Projections.Albums;

public sealed record AlbumProjection(
    Guid Guid,
    string Title,
    Guid CreatedBy,
    string CreatedByName,
    DateTime CreatedAt,
    DateTime ModifiedAt,
    string ThumbnailSource,
    string ThumbnailId,
    string SourceId,
    bool IsFavorite,
    int SongCount,
    int ExpectedCount,
    ArtistInfoProjection? ArtistProjection);