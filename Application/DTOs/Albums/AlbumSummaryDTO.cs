using Application.DTOs.Artists;
using Domain.Entities;
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
    public static AlbumSummaryDTO Create(Album album, Artist artist, bool isFavorite, int songCount)
    {
        return new AlbumSummaryDTO(
            album.Guid,
            album.Title,
            ThumbnailUrl: album.ThumbnailSource is GlobalVariables.PlaylistSource.YouTube or GlobalVariables.PlaylistSource.YouTubeMusic
                ? YoutubeHelper.GetYoutubePlaylistThumbnail(album.ThumbnailId, album.SourceId)
                : "",
            IsFavorite: isFavorite,
            Artist: ArtistInfoDTO.Create(artist),
            SongCount: songCount,
            ExpectedCount: album.ExpectedSongs,
            SourceUrl: YoutubeHelper.GetYoutubeAlbumUrl(album.SourceId)
        );
    }
};