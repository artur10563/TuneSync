using Application.DTOs.Songs;
using Application.Services;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Songs.Query.GetSongFromDb
{
    public sealed record GetSongFromDbCommand(string query, Guid? CurrentUserGuid = null) : IRequest<Result<List<SongDTO>>>;
}
