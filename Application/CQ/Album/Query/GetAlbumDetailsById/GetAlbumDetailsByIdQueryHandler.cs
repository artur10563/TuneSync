using Application.DTOs.Albums;
using Application.Projections;
using Application.Repositories.Shared;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Album.Query.GetAlbumDetailsById;

internal sealed class GetAlbumDetailsByIdQueryHandler : IRequestHandler<GetAlbumDetailsByIdQuery, Result<AlbumDTO>>
{
    private readonly IUnitOfWork _uow;
    private readonly IProjectionProvider _projectionProvider;

    public GetAlbumDetailsByIdQueryHandler(IUnitOfWork uow, IProjectionProvider projectionProvider)
    {
        _uow = uow;
        _projectionProvider = projectionProvider;
    }
    
    
    public async Task<Result<AlbumDTO>> Handle(GetAlbumDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var userGuid = request.UserGuid ?? Guid.Empty;

        var albumInfo = _uow.AlbumRepository
            .NoTrackingQueryable()
            .Where(x => x.Guid == request.AlbumGuid)
            .Select(_projectionProvider.GetAlbumWithArtistProjection(userGuid))
            .FirstOrDefault();
        
        if (albumInfo == null)
            return Error.NotFound(nameof(Album));

        return AlbumDTO.FromProjection(albumInfo);
    }
}