using System.Linq.Expressions;
using Application.Projections;
using Application.Projections.Artists;
using Domain.Entities;

namespace Infrastructure.Projections;

public static partial class ProjectionHelper
{
    public static Expression<Func<Artist, ArtistInfoProjection>> GetArtistInfoProjection()
    {
        return artist => new ArtistInfoProjection(
            artist.Guid,
            artist.Name,
            artist.DisplayName,
            artist.YoutubeChannelId,
            artist.ThumbnailUrl
        );
    }
    
    public static Expression<Func<Artist, ArtistInfoWithCountsProjection>> GetArtistInfoWithCountsProjection()
    {
        return artist => new ArtistInfoWithCountsProjection(
            artist.Guid,
            artist.Name,
            artist.DisplayName,
            artist.YoutubeChannelId,
            artist.ThumbnailUrl,
            artist.TopLvlParent.Name,
            artist.Songs.Count,
            artist.Albums.Count,
            artist.Children.Count
        );
    }
}