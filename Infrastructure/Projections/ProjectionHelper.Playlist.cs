using System.Linq.Expressions;
using Application.DTOs.Songs;
using Application.Projections.Playlists;
using Domain.Entities;
using Domain.Primitives;
using LinqKit;

namespace Infrastructure.Projections;

public static partial class ProjectionHelper
{
    public static Expression<Func<Playlist, PlaylistProjection>> GetPlaylistWithSongsProjection(Guid userGuid)
    {
        var songProjection = GetSongWithArtistProjection(userGuid);
        
        Expression<Func<Playlist, PlaylistProjection>> expr = playlist => new PlaylistProjection(
            playlist.Guid,
            playlist.Title,
            playlist.CreatedBy,
            playlist.User.Name ?? string.Empty,
            playlist.CreatedAt,
            playlist.ModifiedAt,
            string.Empty,
            playlist.FavoredBy.Any(x => x.UserGuid == userGuid && x.IsFavorite), 
            playlist.Songs.Count,
            playlist.Songs
                .OrderBy(x => x.CreatedAt)
                .Select(s => songProjection.Invoke(s))
                .ToList()
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
            playlist.Songs.Count
            );
        
        return expr;
    }

}