using System.Linq.Expressions;
using Application.Projections.Albums;
using Application.Projections.Artists;
using Application.Projections.Playlists;
using Application.Projections.Songs;
using Domain.Entities;

namespace Application.Projections;

public interface IProjectionProvider
{
    Expression<Func<Song, SongProjection>> GetSongWithArtistProjection(Guid userGuid);
    
    
    Expression<Func<Album, AlbumProjection>> GetAlbumWithArtistProjection(Guid userGuid);
    Expression<Func<Album, AlbumSummaryProjection>> GetAlbumSummaryProjection(Guid userGuid);
    
    
    Expression<Func<Artist, ArtistInfoProjection>> GetArtistInfoProjection();
    Expression<Func<Artist, ArtistInfoWithCountsProjection>> GetArtistInfoWithCountsProjection();
    
    
    Expression<Func<Playlist, PlaylistProjection>> GetPlaylistProjection(Guid userGuid, int page = 1);
    Expression<Func<Playlist, PlaylistSummaryProjection>> GetPlaylistSummaryProjection(Guid userGuid);
}