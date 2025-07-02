using Application.DTOs.Artists;
using Application.Projections.Songs;
using Domain.Helpers;
using static Domain.Primitives.GlobalVariables;

namespace Application.DTOs.Songs
{
    public record SongDTO(
        Guid Guid,
        string Title,
        DateTime CreatedAt,
        string Source, //Youtube, deezer, upload from PC, etc
        string SourceUrl, //Link to YT video, etc
        string ThumbnailUrl,
        string AudioPath,
        int AudioSize,
        TimeSpan AudioLength,
        ArtistInfoDTO Artist,
        Guid? AlbumGuid,
        string? Album,
        bool IsFavorite
    )
    {
        public static SongDTO FromProjection(SongProjection projection)
        {
            return new SongDTO
            (
                projection.Guid,
                projection.Title,
                projection.CreatedAt,
                projection.Source,
                SourceUrl: projection.Source == SongSource.YouTube ? YoutubeHelper.GetYoutubeChannel(projection.SourceId!) : "",
                YoutubeHelper.GetYoutubeThumbnail(projection.SourceId),
                GetFirebaseMP3Link(projection.AudioPath),
                projection.AudioSize,
                projection.AudioLength,
                ArtistInfoDTO.FromProjection(projection.Artist),
                projection.AlbumGuid,
                projection.Album,
                projection.IsFavorite
            );
        }
    }
}