using Application.DTOs.Songs;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Songs.Query.GetSongFromDb
{
    public sealed record GetSongFromDbCommand(string query) : IRequest<Result<List<SongDTO>>>;
}
