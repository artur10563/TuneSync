using Application.DTOs;
using Application.DTOs.Artists;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Artists.Query.GetArtistList;

public sealed record GetArtistListQuery(string? Query, bool CollapseChildren, int Page, int PageSize, string OrderBy, bool IsDescending) 
    : IPaged, IRequest<PaginatedResult<IEnumerable<ArtistInfoWithCountsDTO>>>;