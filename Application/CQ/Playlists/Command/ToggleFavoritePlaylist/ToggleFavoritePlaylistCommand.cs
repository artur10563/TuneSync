using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists.Command.ToggleFavoritePlaylist;

public record ToggleFavoritePlaylistCommand(Guid PlaylistGuid = default, Guid UserGuid = default) : IRequest<Result>;