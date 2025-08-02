using System.Linq.Expressions;
using Application.DTOs.Songs;
using Application.Projections;
using Application.Projections.Albums;
using Application.Projections.Artists;
using Application.Projections.Playlists;
using Application.Projections.Songs;
using Domain.Entities;
using Domain.Primitives;

namespace Infrastructure.Projections;

public class ProjectionProvider : IProjectionProvider
{
    public Expression<Func<Song, SongProjection>> GetSongWithArtistProjection(Guid userGuid) =>
        ProjectionHelper.GetSongWithArtistProjection(userGuid);


    public Expression<Func<Album, AlbumProjection>> GetAlbumWithArtistProjection(Guid userGuid) =>
        ProjectionHelper.GetAlbumWithArtistProjection(userGuid);

    public Expression<Func<Artist, ArtistInfoProjection>> GetArtistInfoProjection() =>
        ProjectionHelper.GetArtistInfoProjection();

    public Expression<Func<Artist, ArtistInfoWithCountsProjection>> GetArtistInfoWithCountsProjection()
        => ProjectionHelper.GetArtistInfoWithCountsProjection();

    public Expression<Func<Album, AlbumSummaryProjection>> GetAlbumSummaryProjection(Guid userGuid) =>
        ProjectionHelper.GetAlbumSummaryProjection(userGuid);

    public Expression<Func<Playlist, PlaylistProjection>> GetPlaylistProjection(Guid userGuid, int page = 1) =>
        ProjectionHelper.GetPlaylistProjection(userGuid, page);

    public Expression<Func<Playlist, PlaylistSummaryProjection>> GetPlaylistSummaryProjection(Guid userGuid) =>
        ProjectionHelper.GetPlaylistSummaryProjection(userGuid);
}