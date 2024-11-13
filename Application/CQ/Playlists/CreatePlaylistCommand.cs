using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists
{
    public sealed record CreatePlaylistCommand(string PlaylistName, Guid CreatedBy) : IRequest<Result<Guid>>;
}
