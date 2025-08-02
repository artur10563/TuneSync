using Application.DTOs;
using Application.DTOs.Songs;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Songs.Query.GetSongFromDb
{
    public sealed record GetSongFromDbCommand(string query, Guid? CurrentUserGuid = null, int Page = 1) 
        : IPaged, IRequest<PaginatedResult<List<SongDTO>>>;
}
