using Application.DTOs.Songs;
using Application.Projections.Playlists;
using Domain.Entities;
using Domain.Primitives;

namespace Application.DTOs.Playlists
{
    public record PlaylistDTO(
        Guid Guid,
        string Title,
        Guid CreatedBy,
        string CreatedByName,
        DateTime CreatedAt,
        DateTime ModifiedAt,
        string ThumbnailUrl,
        bool IsFavorite,
        int SongCount,
        List<SongDTO> Songs
    )
    {
        public static PlaylistDTO FromProjection(PlaylistProjection projection)
        {
            return new PlaylistDTO(
                projection.Guid,
                projection.Title,
                projection.CreatedBy,
                projection.CreatedByName,
                projection.CreatedAt,
                projection.ModifiedAt,
                projection.ThumbnailUrl,
                projection.IsFavorite,
                projection.SongCount,
                projection.Songs.Select(SongDTO.FromProjection).ToList()
            );
        }
    }
}