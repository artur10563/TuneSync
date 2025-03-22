using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists.Command.DeletePlaylist;

public sealed record DeletePlaylistCommand(Guid PlaylistGuid, Guid UserGuid) : IRequest<Result>;