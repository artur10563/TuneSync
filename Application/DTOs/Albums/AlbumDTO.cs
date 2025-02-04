using Application.DTOs.Songs;
using Domain.Entities;
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
    PaginatedResponse<ICollection<SongDTO>> Songs)
{
    public static AlbumDTO Create(Album album, List<SongDTO> songs,  PageInfo pageInfo, bool isFavorite, int songCount)
    {
        return new AlbumDTO(
            album.Guid,
            album.Title,
            album.CreatedBy,
            CreatedByName: album?.User.Name ?? string.Empty,
            album.CreatedAt,
            album.ModifiedAt,
            ThumbnailUrl: album.ThumbnailSource == GlobalVariables.PlaylistSource.YouTube
                ? GlobalVariables.GetYoutubePlaylistThumbnail(album.ThumbnailId)
                : "",
            Songs: new PaginatedResponse<ICollection<SongDTO>>(songs, pageInfo),
            IsFavorite: isFavorite, 
            SongCount: songCount
        );
    }
};