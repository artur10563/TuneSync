using Application.Projections.Artists;

namespace Application.Projections.Albums;

public record AlbumSummaryProjection(
    Guid Guid,
    string Title,
    string ThumbnailSource,
    string ThumbnailId,
    string SourceId,
    bool IsFavorite,
    int SongCount,
    int ExpectedCount,
    ArtistInfoProjection? Artist);
