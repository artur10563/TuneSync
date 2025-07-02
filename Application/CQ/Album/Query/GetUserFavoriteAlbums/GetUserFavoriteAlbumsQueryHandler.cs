using Application.DTOs.Albums;
using Application.Projections;
using Application.Repositories.Shared;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Album.Query.GetUserFavoriteAlbums;

public class GetUserFavoriteAlbumsQueryHandler : IRequestHandler<GetUserFavoriteAlbumsQuery, Result<IEnumerable<AlbumSummaryDTO>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IProjectionProvider _projectionProvider;
    
    public GetUserFavoriteAlbumsQueryHandler(IUnitOfWork uow, IProjectionProvider projectionProvider)
    {
        _uow = uow;
        _projectionProvider = projectionProvider;
    }

    public async Task<Result<IEnumerable<AlbumSummaryDTO>>> Handle(GetUserFavoriteAlbumsQuery request, CancellationToken cancellationToken)
    {
        if (request.UserGuid == Guid.Empty)
            return Error.NotFound(nameof(Album));

        var userFA = _uow.AlbumRepository.NoTrackingQueryable()
            .Where(p => p.FavoredBy.Any(ufp => ufp.UserGuid == request.UserGuid))
            .OrderBy(x => x.CreatedAt)
            .Select(_projectionProvider.GetAlbumSummaryProjection(request.UserGuid))
            .Select(x => AlbumSummaryDTO.FromProjection(x))
            .ToList();
        
        return userFA;
    }
}