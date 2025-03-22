using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists.Command.CreatePlaylistFromYoutube;

public sealed record CreatePlaylistFromYoutubeCommand(string PlaylistId, Guid CreatedBy) : IRequest<Result<string>>;