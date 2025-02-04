using Application.DTOs.Playlists;
using Application.Repositories.Shared;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists.Query.GetUserFavoritePlaylists;

public class GetUserFavoritePlaylistsQueryHandler : IRequestHandler<GetUserFavoritePlaylistsQuery, Result<IEnumerable<PlaylistSummaryDTO>>>
{
    private readonly IUnitOfWork _uow;

    public GetUserFavoritePlaylistsQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<IEnumerable<PlaylistSummaryDTO>>> Handle(GetUserFavoritePlaylistsQuery request, CancellationToken cancellationToken)
    {
        if (request.UserGuid == Guid.Empty)
            return Error.NotFound(nameof(Playlist));

        var userFP = _uow.PlaylistRepository
            .Where(a => a.FavoredBy
                    .Any(ufp => ufp.UserGuid == request.UserGuid && ufp.IsFavorite),
                asNoTracking: true,
                includes: [p => p.FavoredBy]
            ).OrderBy(x=>x.CreatedAt).Select(playlist => PlaylistSummaryDTO.Create(playlist, true, playlist.Songs.Count())).ToList();

        return userFP;
    }
}