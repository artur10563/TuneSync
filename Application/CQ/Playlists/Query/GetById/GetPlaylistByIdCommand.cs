using Application.DTOs.Playlists;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists.Query.GetById
{

    public sealed record GetPlaylistByIdCommand(Guid PlaylistGuid, Guid? UserGuid = null) : IRequest<Result<PlaylistDTO>>;
}
