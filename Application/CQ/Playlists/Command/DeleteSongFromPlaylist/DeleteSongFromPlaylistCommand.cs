using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists.Command.DeleteSongFromPlaylist;

public record DeleteSongFromPlaylistCommand(Guid PlaylistGuid, Guid SongGuid, Guid CurrentUserGuid) : IRequest<Result>;