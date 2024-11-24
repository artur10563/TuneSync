using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists.Command.Create
{
    public sealed record AddSongToPlaylistCommand(Guid PlaylistGuid, Guid SongGuid, Guid CurrentUserGuid) : IRequest<Result<Guid>>;
}
