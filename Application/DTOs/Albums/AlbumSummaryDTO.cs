using Application.DTOs.Artists;
using Application.Projections.Albums;
using Domain.Helpers;
using Domain.Primitives;

namespace Application.DTOs.Albums;

public sealed record AlbumSummaryDTO(
    Guid Guid,
    string Title,
    string ThumbnailUrl,
    bool IsFavorite,
    ArtistInfoDTO? Artist,
    int SongCount,
    int ExpectedCount,
    string SourceUrl)
{
    public static AlbumSummaryDTO FromProjection(AlbumSummaryProjection projection)
    {
        return new AlbumSummaryDTO(
            projection.Guid,
            projection.Title,
            ThumbnailUrl: projection.ThumbnailSource switch
            {
                GlobalVariables.PlaylistSource.YouTube or GlobalVariables.PlaylistSource.YouTubeMusic => 
                    YoutubeHelper.GetYoutubePlaylistThumbnail(projection.ThumbnailId, projection.SourceId),
                _ => ""
            },
            IsFavorite: projection.IsFavorite,
            Artist: ArtistInfoDTO.FromProjection(projection.Artist),
            SongCount: projection.SongCount,
            ExpectedCount: projection.ExpectedCount,
            SourceUrl: YoutubeHelper.GetYoutubeAlbumUrl(projection.SourceId)
        );
    }
};