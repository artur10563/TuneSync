using Application.DTOs.Artists;
using Application.DTOs.Songs;
using Domain.Entities;
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
    public static AlbumDTO Create(Album album, Artist artist, List<SongDTO> songs,  PageInfo pageInfo, bool isFavorite, int songCount)
    {
        return new AlbumDTO(
            album.Guid,
            album.Title,
            album.CreatedBy,
            CreatedByName: album?.User.Name ?? string.Empty,
            album.CreatedAt,
            album.ModifiedAt,
            ThumbnailUrl: album.ThumbnailSource switch
            {
                GlobalVariables.PlaylistSource.YouTube or GlobalVariables.PlaylistSource.YouTubeMusic => 
                    YoutubeHelper.GetYoutubePlaylistThumbnail(album.ThumbnailId, album.SourceId),
                _ => ""
            },
            Songs: new PaginatedResponse<ICollection<SongDTO>>(songs, pageInfo),
            IsFavorite: isFavorite, 
            ExpectedCount: album.ExpectedSongs,
            Artist: ArtistInfoDTO.Create(artist), 
            SongCount: songCount,
            SourceUrl: YoutubeHelper.GetYoutubeAlbumUrl(album.SourceId)
        );
    }
};