using Application.DTOs.Artists;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Album.Query.GetArtistList;

public sealed record GetArtistListQuery(string? Query, bool CollapseChildren, int Page, int PageSize, string OrderBy, bool IsDescending) 
    : IRequest<PaginatedResult<IEnumerable<ArtistInfoWithCountsDTO>>>;