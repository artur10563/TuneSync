using Application.Projections.Artists;
using Domain.Helpers;

namespace Application.DTOs.Artists
{
    public record ArtistInfoDTO(
        Guid Guid,
        string Name,
        string DisplayName,
        string ChannelUrl,
        string? ThumbnailUrl
    )
    {
        public static ArtistInfoDTO FromProjection(ArtistInfoProjection projection)
        {
            return new ArtistInfoDTO(
                projection.Guid,
                projection.Name,
                projection.DisplayName,
                YoutubeHelper.GetYoutubeChannel(projection.YoutubeChannelId),
                projection.ThumbnailUrl
            );
        }
    }
}