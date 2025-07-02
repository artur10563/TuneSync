using Application.Projections.Songs;

namespace Application.Projections.Playlists;

public record PlaylistProjection(
    Guid Guid,
    string Title,
    Guid CreatedBy,
    string CreatedByName,
    DateTime CreatedAt,
    DateTime ModifiedAt,
    string ThumbnailUrl,
    bool IsFavorite,
    int SongCount,
    List<SongProjection> Songs
    );