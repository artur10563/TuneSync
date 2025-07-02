using Application.DTOs.Songs;
using Application.Projections;
using Application.Repositories.Shared;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Songs.Query.GetUserFavoriteSongs;

public class GetUserFavoriteSongsQueryHandler : IRequestHandler<GetUserFavoriteSongsQuery, Result<IEnumerable<SongDTO>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IProjectionProvider _projectionProvider;
    
    public GetUserFavoriteSongsQueryHandler(IUnitOfWork uow, IProjectionProvider projectionProvider)
    {
        _uow = uow;
        _projectionProvider = projectionProvider;
    }

    public async Task<Result<IEnumerable<SongDTO>>> Handle(GetUserFavoriteSongsQuery request, CancellationToken cancellationToken)
    {
        if (request.UserGuid == Guid.Empty)
            return Error.NotFound(nameof(Song));
        
        var ufs = _uow.SongRepository.NoTrackingQueryable()
            .Where(x => x.FavoredBy.Any(us => us.UserGuid == request.UserGuid && us.IsFavorite))
            .OrderBy(x => x.CreatedAt)
            .Select(_projectionProvider.GetSongWithArtistProjection(request.UserGuid))
            .Select(x => SongDTO.FromProjection(x))
            .ToList();

        return ufs;
    }
}