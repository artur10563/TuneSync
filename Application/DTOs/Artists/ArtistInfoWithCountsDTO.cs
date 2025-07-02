using Application.Projections.Artists;
using Domain.Helpers;

namespace Application.DTOs.Artists;

public record ArtistInfoWithCountsDTO(
    Guid Guid,
    string Name,
    string DisplayName,
    string ChannelUrl,
    string? ThumbnailUrl,
    string? ParentName, //TODO: Add minimal Guid, Name object if will ever need
    int SongCount,
    int AlbumCount,
    int ChildrenCount)
{

    public static ArtistInfoWithCountsDTO FromProjection(ArtistInfoWithCountsProjection projection)
    {
        return new ArtistInfoWithCountsDTO(
            projection.Guid,
            projection.Name,
            projection.DisplayName,
            YoutubeHelper.GetYoutubeChannel(projection.ChannelId),
            projection.ThumbnailUrl,
            projection.ParentName,
            projection.SongCount,
            projection.AlbumCount,
            projection.ChildrenCount
        );
    }
};