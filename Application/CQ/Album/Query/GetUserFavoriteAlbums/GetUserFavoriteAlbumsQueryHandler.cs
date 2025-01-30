using Application.DTOs.Playlists;
using Application.Repositories.Shared;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Album.Query.GetUserFavoriteAlbums;

public class GetUserFavoriteAlbumsQueryHandler : IRequestHandler<GetUserFavoriteAlbumsQuery, Result<List<PlaylistSummaryDTO>>>
{
    private readonly IUnitOfWork _uow;

    public GetUserFavoriteAlbumsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<List<PlaylistSummaryDTO>>> Handle(GetUserFavoriteAlbumsQuery request, CancellationToken cancellationToken)
    {
        if (request.UserGuid == Guid.Empty)
            return Error.NotFound(nameof(Album));

        var userFA = _uow.AlbumRepository
            .Where(a => a.FavoredBy
                    .Any(ufa => ufa.UserGuid == request.UserGuid && ufa.IsFavorite),
                includes: [a => a.FavoredBy]
            ).OrderBy(x=>x.CreatedAt).Select(album => PlaylistSummaryDTO.Create(album, true)).ToList();
        
        return userFA;
    }
}