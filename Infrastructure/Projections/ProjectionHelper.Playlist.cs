using System.Linq.Expressions;
using Application.DTOs.Songs;
using Application.Extensions;
using Application.Projections.Playlists;
using Domain.Entities;
using Domain.Primitives;
using LinqKit;

namespace Infrastructure.Projections;

public static partial class ProjectionHelper
{
    public static Expression<Func<Playlist, PlaylistProjection>> GetPlaylistProjection(Guid userGuid, int page)
    {
        Expression<Func<Playlist, PlaylistProjection>> expr = playlist => new PlaylistProjection(
            playlist.Guid,
            playlist.Title,
            playlist.CreatedBy,
            playlist.User.Name ?? string.Empty,
            playlist.CreatedAt,
            playlist.ModifiedAt,
            string.Empty,
            playlist.FavoredBy.Any(x => x.UserGuid == userGuid && x.IsFavorite), 
            playlist.PlaylistSongs.Count
        );

        return expr.Expand();
    }

    public static Expression<Func<Playlist, PlaylistSummaryProjection>> GetPlaylistSummaryProjection(Guid userGuid)
    {
        Expression<Func<Playlist, PlaylistSummaryProjection>> expr = playlist => new PlaylistSummaryProjection(
            playlist.Guid,
            playlist.Title,
            string.Empty,
            playlist.FavoredBy.Any(x => x.UserGuid == userGuid && x.IsFavorite),
            playlist.PlaylistSongs.Count
            );
        
        return expr;
    }

}