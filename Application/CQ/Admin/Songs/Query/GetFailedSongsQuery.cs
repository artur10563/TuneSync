using Application.DTOs;
using Application.DTOs.Songs;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Admin.Songs.Query;

public record GetFailedSongsQuery(Guid UserGuid, int Page = 1) 
    : IPaged, IRequest<PaginatedResult<IEnumerable<SongDTO>>>;