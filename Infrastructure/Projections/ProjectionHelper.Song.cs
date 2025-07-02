using System.Linq.Expressions;
using Application.Projections;
using Application.Projections.Songs;
using Domain.Entities;
using LinqKit;

namespace Infrastructure.Projections;

public static partial class ProjectionHelper
{
    public static Expression<Func<Song, SongProjection>> GetSongWithArtistProjection(Guid userGuid)
    {
        var artistProjection = GetArtistInfoProjection();

        Expression<Func<Song, SongProjection>> expr = song => new SongProjection(
            song.Guid,
            song.Title,
            song.CreatedAt,
            song.Source,
            song.SourceId,
            song.AudioPath,
            song.AudioSize,
            song.AudioLength,
            artistProjection.Invoke(song.Artist),
            song.AlbumGuid,
            song.Album != null ? song.Album.Title : null,
            song.FavoredBy.Any(us => us.UserGuid == userGuid && us.IsFavorite)
        );

        return expr.Expand();
    }
}