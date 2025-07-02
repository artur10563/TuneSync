using System.Linq.Expressions;
using Application.DTOs.Albums;
using Application.DTOs.Artists;
using Application.Projections;
using Application.Projections.Albums;
using Domain.Entities;
using Domain.Helpers;
using Domain.Primitives;
using LinqKit;

namespace Infrastructure.Projections;

public static partial class ProjectionHelper
{
    public static Expression<Func<Album, AlbumProjection>> GetAlbumWithArtistProjection(Guid userGuid)
    {
        var artistProjection = GetArtistInfoProjection();

        Expression<Func<Album, AlbumProjection>> expr = album => new AlbumProjection(
            album.Guid,
            album.Title,
            album.CreatedBy,
            album.User.Name ?? string.Empty,
            album.CreatedAt,
            album.ModifiedAt,
            album.ThumbnailSource,
            album.ThumbnailId,
            album.SourceId,
            album.FavoredBy.Any(x => x.UserGuid == userGuid && x.IsFavorite),
            album.Songs.Count,
            album.ExpectedSongs,
            artistProjection.Invoke(album.Artist)
        );

        return expr.Expand();
    }

    public static Expression<Func<Album, AlbumSummaryProjection>> GetAlbumSummaryProjection(Guid userGuid)
    {
        var artistProjection = GetArtistInfoProjection();

        Expression<Func<Album, AlbumSummaryProjection>> expr = album => new AlbumSummaryProjection(
            album.Guid,
            album.Title,
            album.ThumbnailSource,
            album.ThumbnailId,
            album.SourceId,
            album.FavoredBy.Any(x => x.UserGuid == userGuid && x.IsFavorite),
            album.Songs.Count,
            album.ExpectedSongs,
            artistProjection.Invoke(album.Artist)
        );

        return expr.Expand();
    }
}