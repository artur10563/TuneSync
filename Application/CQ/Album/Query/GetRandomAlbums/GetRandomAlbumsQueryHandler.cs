using Application.DTOs.Albums;
using Application.Extensions;
using Application.Repositories.Shared;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Album.Query.GetRandomAlbums;

public sealed class GetRandomAlbumsQueryHandler : IRequestHandler<GetRandomAlbumsQuery, Result<IEnumerable<AlbumSummaryDTO>>>
{
    private readonly IUnitOfWork _uow;

    public GetRandomAlbumsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<IEnumerable<AlbumSummaryDTO>>> Handle(GetRandomAlbumsQuery request, CancellationToken cancellationToken)
    {
        var result = (
            from album in _uow.AlbumRepository.NoTrackingQueryable()
            join artist in _uow.ArtistRepository.NoTrackingQueryable()
                on album.ArtistGuid equals artist.Guid
            join fa in _uow.UserFavoriteAlbumRepository.NoTrackingQueryable()
                on new { userGuid = request.UserGuid, albumGuid = album.Guid } equals new { userGuid = fa.UserGuid, albumGuid = fa.AlbumGuid } into favJoin
            from fa in favJoin.DefaultIfEmpty()
            join song in _uow.SongRepository.NoTrackingQueryable()
                on album.Guid equals song.AlbumGuid into songGroup
            select AlbumSummaryDTO.Create(album, artist, fa != null && fa.IsFavorite, songGroup.Count()
            )
        ).Page(1).ToList();

        return result;
    }
}