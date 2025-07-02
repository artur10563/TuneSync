using Application.DTOs.Playlists;
using Application.Projections;
using Application.Repositories.Shared;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists.Query.GetUserFavoritePlaylists;

public class GetUserFavoritePlaylistsQueryHandler : IRequestHandler<GetUserFavoritePlaylistsQuery, Result<IEnumerable<PlaylistSummaryDTO>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IProjectionProvider _projectionProvider;
    
    public GetUserFavoritePlaylistsQueryHandler(IUnitOfWork uow, IProjectionProvider projectionProvider)
    {
        _uow = uow;
        _projectionProvider = projectionProvider;
    }

    public async Task<Result<IEnumerable<PlaylistSummaryDTO>>> Handle(GetUserFavoritePlaylistsQuery request, CancellationToken cancellationToken)
    {
        if (request.UserGuid == Guid.Empty)
            return Error.NotFound(nameof(Playlist));
        
        var userFP = _uow.PlaylistRepository.NoTrackingQueryable()
            .Where(p => p.FavoredBy.Any(ufp => ufp.UserGuid == request.UserGuid))
            .OrderBy(x => x.CreatedAt)
            .Select(_projectionProvider.GetPlaylistSummaryProjection(request.UserGuid))
            .Select(PlaylistSummaryDTO.FromProjection)
            .ToList();
            
        return userFP;
    }
}