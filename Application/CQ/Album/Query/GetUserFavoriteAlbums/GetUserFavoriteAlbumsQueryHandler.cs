using Application.DTOs.Albums;
using Application.Repositories.Shared;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Album.Query.GetUserFavoriteAlbums;

public class GetUserFavoriteAlbumsQueryHandler : IRequestHandler<GetUserFavoriteAlbumsQuery, Result<List<AlbumSummaryDTO>>>
{
    private readonly IUnitOfWork _uow;

    public GetUserFavoriteAlbumsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<List<AlbumSummaryDTO>>> Handle(GetUserFavoriteAlbumsQuery request, CancellationToken cancellationToken)
    {
        if (request.UserGuid == Guid.Empty)
            return Error.NotFound(nameof(Album));

        var userFA =
        (
            from album in _uow.AlbumRepository.NoTrackingQueryable()
            join artist in _uow.ArtistRepository.NoTrackingQueryable()
                on album.ArtistGuid equals artist.Guid
            join fb in _uow.UserFavoriteAlbumRepository.NoTrackingQueryable()
                on new { a = album.Guid, u = request.UserGuid } equals new { a = fb.AlbumGuid, u = fb.UserGuid } into favAlbumJoin
            from favAlbum in favAlbumJoin.DefaultIfEmpty()
            join song in _uow.SongRepository.NoTrackingQueryable()
                on album.Guid equals song.AlbumGuid into songGroup
            where favAlbum.IsFavorite
            orderby album.CreatedBy ascending
            select AlbumSummaryDTO.Create(album, artist, true, songGroup.Count())
        ).ToList();

        return userFA;
    }
}