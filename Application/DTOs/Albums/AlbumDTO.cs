using Application.DTOs.Songs;
using Domain.Primitives;

namespace Application.DTOs.Albums;

public sealed record AlbumDTO(
    Guid Guid,
    string Title,
    Guid CreatedBy,
    string CreatedByName,
    DateTime CreatedAt,
    DateTime ModifiedAt,
    string ThumbnailUrl,
    bool IsFavorite,
    int SongCount,
    PaginatedResponse<ICollection<SongDTO>> Songs)
{
};