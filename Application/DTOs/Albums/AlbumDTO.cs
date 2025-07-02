using Application.DTOs.Artists;
using Application.DTOs.Songs;
using Application.Projections.Albums;
using Domain.Helpers;
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
    int ExpectedCount,
    string SourceUrl,
    ArtistInfoDTO? Artist,
    PaginatedResponse<ICollection<SongDTO>> Songs)
{
    public static AlbumDTO FromProjection(AlbumProjection projection, PaginatedResponse<ICollection<SongDTO>> songsPage)
    {
        var thumbnailUrl = projection.ThumbnailSource switch
        {
            GlobalVariables.PlaylistSource.YouTube or GlobalVariables.PlaylistSource.YouTubeMusic =>
                YoutubeHelper.GetYoutubePlaylistThumbnail(projection.ThumbnailId, projection.SourceId),
            _ => ""
        };
        
        
        return new AlbumDTO(
            projection.Guid,
            projection.Title,
            projection.CreatedBy,
            projection.CreatedByName,
            projection.CreatedAt,
            projection.ModifiedAt,
            thumbnailUrl,
            projection.IsFavorite,
            projection.SongCount,
            projection.ExpectedCount,
            YoutubeHelper.GetYoutubeAlbumUrl(projection.SourceId),
            ArtistInfoDTO.FromProjection(projection.ArtistProjection),
            songsPage
        );
    }
};