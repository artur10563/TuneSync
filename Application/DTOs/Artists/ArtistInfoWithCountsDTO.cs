using Domain.Entities;
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

    public static ArtistInfoWithCountsDTO Create(Artist artist)
    {
        return new ArtistInfoWithCountsDTO(
            artist.Guid,
            artist.Name,
            artist.DisplayName,
            YoutubeHelper.GetYoutubeChannel(artist.YoutubeChannelId),
            artist.ThumbnailUrl,
            artist.TopLvlParent?.Name,
            artist.Songs.Count,
            artist.Albums.Count,
            artist.AllChildren.Count
        );
    }

    public static ArtistInfoWithCountsDTO Create(Artist artist, int songCount, int albumCount, int childrenCount)
    {
        return new ArtistInfoWithCountsDTO(
            artist.Guid,
            artist.Name,
            artist.DisplayName,
            YoutubeHelper.GetYoutubeChannel(artist.YoutubeChannelId),
            artist.ThumbnailUrl,
            artist.Parent?.Name,
            songCount,
            albumCount,
            childrenCount
        );
    }
};