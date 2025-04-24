using Application.DTOs.Artists;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Album.Query.GetArtistList;

public sealed record GetArtistListQuery(string? Query, int Page, int PageSize, string OrderBy, bool IsDescending) 
    : IRequest<PaginatedResult<IEnumerable<ArtistInfoDTO>>>;