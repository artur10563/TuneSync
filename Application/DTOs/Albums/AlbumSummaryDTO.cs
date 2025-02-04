using Application.DTOs.Artists;
using Domain.Entities;
using Domain.Primitives;

namespace Application.DTOs.Albums;

public sealed record AlbumSummaryDTO(
    Guid Guid,
    string Title,
    string ThumbnailUrl,
    bool IsFavorite,
    ArtistInfoDTO? Artist,
    int SongCount)
{
    public static AlbumSummaryDTO Create(Album album, Artist artist, bool isFavorite, int songCount)
    {
        return new AlbumSummaryDTO(
            album.Guid,
            album.Title,
            ThumbnailUrl: album.ThumbnailSource == GlobalVariables.PlaylistSource.YouTube
                ? GlobalVariables.GetYoutubePlaylistThumbnail(album.ThumbnailId)
                : "",
            IsFavorite: isFavorite,
            Artist: ArtistInfoDTO.Create(artist),
            SongCount: songCount
        );
    }
    
    //Probably will delete
    // public static List<AlbumSummaryDTO> Create(ICollection<Album> album, Guid userGuid)
    // {
    //     return album.Select(a =>
    //     {
    //         var v = a.FavoredBy.ToList();
    //         var isFav = a.FavoredBy.Any(x => x.UserGuid == userGuid && x.IsFavorite && x.AlbumGuid == a.Guid);
    //         return Create(a, isFav, 0);
    //     }).ToList();
    // }
};