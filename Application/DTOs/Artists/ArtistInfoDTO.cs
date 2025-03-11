using Domain.Entities;
using Domain.Primitives;

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
        public static ArtistInfoDTO Create(Artist artist)
        {
            return new ArtistInfoDTO(
                artist.Guid,
                artist.Name,
                artist.DisplayName,
                GlobalVariables.GetYoutubeChannel(artist.YoutubeChannelId),
                artist.ThumbnailUrl
            );
        }
    }
}