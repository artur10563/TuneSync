using Application.DTOs.Playlists;
using Application.Repositories.Shared;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists.Query.GetUserFavoritePlaylists;

public class GetUserFavoritePlaylistsQueryHandler : IRequestHandler<GetUserFavoritePlaylistsQuery, Result<List<PlaylistSummaryDTO>>>
{
    private readonly IUnitOfWork _uow;

    public GetUserFavoritePlaylistsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<List<PlaylistSummaryDTO>>> Handle(GetUserFavoritePlaylistsQuery request, CancellationToken cancellationToken)
    {
        if (request.UserGuid == Guid.Empty)
            return Error.NotFound(nameof(Playlist));

        var userFP = _uow.PlaylistRepository
            .Where(a => a.FavoredBy
                    .Any(ufp => ufp.UserGuid == request.UserGuid && ufp.IsFavorite),
                includes: [p => p.FavoredBy]
            ).OrderBy(x=>x.CreatedAt).Select(album => PlaylistSummaryDTO.Create(album, true)).ToList();

        return userFP;
    }
}