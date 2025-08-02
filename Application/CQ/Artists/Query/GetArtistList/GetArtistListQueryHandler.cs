using Application.DTOs.Artists;
using Application.Extensions;
using Application.Projections;
using Application.Repositories.Shared;
using Domain.Primitives;
using FluentValidation;
using MediatR;

namespace Application.CQ.Artists.Query.GetArtistList;

public class GetArtistListQueryHandler : IRequestHandler<GetArtistListQuery, PaginatedResult<IEnumerable<ArtistInfoWithCountsDTO>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IValidator<GetArtistListQuery> _validator;
    private readonly IProjectionProvider _projectionProvider;

    public GetArtistListQueryHandler(IUnitOfWork uow, IValidator<GetArtistListQuery> validator, IProjectionProvider projectionProvider)
    {
        _uow = uow;
        _validator = validator;
        _projectionProvider = projectionProvider;
    }

    public async Task<PaginatedResult<IEnumerable<ArtistInfoWithCountsDTO>>> Handle(GetArtistListQuery request, CancellationToken cancellationToken)
    {
        var validationErrors = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationErrors.IsValid)
            return validationErrors.AsErrors(request.Page);

        var query = _uow.ArtistRepository.NoTrackingQueryable();

        if (!string.IsNullOrEmpty(request.Query))
        {
            var isGuidSearch = Guid.TryParse(request.Query, out var parsedGuid);
            var filter = request.Query.ToLower();
            query = query.Where(x =>
                x.Name.ToLower().Contains(filter) ||
                (isGuidSearch && x.Guid == parsedGuid));
        }

        //Only return artists who don't have parent
        if (request.CollapseChildren)
        {
            query = query.Where(x => x.ParentId == null && x.TopLvlParentId == null);
        }

        query = _uow.ApplyOrdering(query, request.OrderBy, request.IsDescending);
        var filteredQuery = query.Select(_projectionProvider.GetArtistInfoWithCountsProjection());
        
        var artists = filteredQuery
            .Page(request.Page, request.PageSize)
            .Select(x => ArtistInfoWithCountsDTO.FromProjection(x))
            .ToList();

        return (artists, request.Page, request.PageSize, query.Count());
    }
}