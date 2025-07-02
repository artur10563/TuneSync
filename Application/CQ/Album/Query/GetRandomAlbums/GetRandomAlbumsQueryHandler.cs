using Application.DTOs.Albums;
using Application.Extensions;
using Application.Projections;
using Application.Repositories.Shared;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Album.Query.GetRandomAlbums;

public sealed class GetRandomAlbumsQueryHandler : IRequestHandler<GetRandomAlbumsQuery, Result<IEnumerable<AlbumSummaryDTO>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IProjectionProvider _projectionProvider;

    public GetRandomAlbumsQueryHandler(IUnitOfWork uow, IProjectionProvider projectionProvider)
    {
        _uow = uow;
        _projectionProvider = projectionProvider;
    }

    public async Task<Result<IEnumerable<AlbumSummaryDTO>>> Handle(GetRandomAlbumsQuery request, CancellationToken cancellationToken)
    {

        var result = _uow.AlbumRepository.NoTrackingQueryable()
            .Select(_projectionProvider.GetAlbumSummaryProjection(request.UserGuid))
            .Page(1)
            .Select(x => AlbumSummaryDTO.FromProjection(x))
            .ToList();
        
        return result;
    }
}