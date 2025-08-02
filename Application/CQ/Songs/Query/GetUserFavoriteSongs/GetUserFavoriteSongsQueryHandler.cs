using Application.DTOs.Songs;
using Application.Extensions;
using Application.Projections;
using Application.Repositories.Shared;
using Domain.Primitives;
using FluentValidation;
using MediatR;

namespace Application.CQ.Songs.Query.GetUserFavoriteSongs;

public class GetUserFavoriteSongsQueryHandler : IRequestHandler<GetUserFavoriteSongsQuery, PaginatedResult<IEnumerable<SongDTO>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IProjectionProvider _projectionProvider;
    private readonly IValidator<GetUserFavoriteSongsQuery> _validator;

    public GetUserFavoriteSongsQueryHandler(IUnitOfWork uow, IProjectionProvider projectionProvider, IValidator<GetUserFavoriteSongsQuery> validator)
    {
        _uow = uow;
        _projectionProvider = projectionProvider;
        _validator = validator;
    }

    public async Task<PaginatedResult<IEnumerable<SongDTO>>> Handle(GetUserFavoriteSongsQuery request, CancellationToken cancellationToken)
    {
        var validationErrors = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationErrors.IsValid)
            return validationErrors.AsErrors(GlobalVariables.PaginationConstants.PageSize);

        var ufsQuery = _uow.SongRepository.NoTrackingQueryable()
            .Where(x => x.FavoredBy.Any(us => us.UserGuid == request.UserGuid && us.IsFavorite));
        
        var ufs = ufsQuery
            .OrderBy(x => x.CreatedAt)
            .Select(_projectionProvider.GetSongWithArtistProjection(request.UserGuid))
            .Page(request.Page)
            .Select(x => SongDTO.FromProjection(x))
            .ToList();
        
        var songsInfo = ufsQuery.GetSongsInfo();

        return (ufs, request.Page, GlobalVariables.PaginationConstants.PageSize, songsInfo.TotalCount, songsInfo.ToMetadataDictionary());
    }
}