using Application.DTOs.Playlists;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists.Query.GetPlaylistsByUser
{
    public sealed record GetPlaylistsByUserCommand(Guid UserGuid) : IRequest<Result<IEnumerable<PlaylistSummaryDTO>>>;
}
