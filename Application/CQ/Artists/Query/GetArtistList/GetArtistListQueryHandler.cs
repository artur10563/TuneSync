using Application.CQ.Album.Query.GetArtistList;
using Application.DTOs.Artists;
using Application.Extensions;
using Application.Repositories.Shared;
using Domain.Primitives;
using FluentValidation;
using MediatR;

namespace Application.CQ.Artists.Query.GetArtistList;

public class GetArtistListQueryHandler : IRequestHandler<GetArtistListQuery, PaginatedResult<IEnumerable<ArtistInfoDTO>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IValidator<GetArtistListQuery> _validator;

    public GetArtistListQueryHandler(IUnitOfWork uow, IValidator<GetArtistListQuery> validator)
    {
        _uow = uow;
        _validator = validator;
    }

    public async Task<PaginatedResult<IEnumerable<ArtistInfoDTO>>> Handle(GetArtistListQuery request, CancellationToken cancellationToken)
    {
        var validationErrors = _validator.Validate(request);
        if (!validationErrors.IsValid)
            return validationErrors.AsErrors(request.Page);

        var query = _uow.ArtistRepository.NoTrackingQueryable();

        var artists = _uow.ApplyOrdering(query, request.OrderBy, request.IsDescending)
            .Page(request.Page, request.PageSize)
            .Select(artist => ArtistInfoDTO.Create(artist))
            .ToList();

        return (artists, request.Page, request.PageSize, _uow.ArtistRepository.Queryable().Count());
    }
}