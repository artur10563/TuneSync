using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists.Query.GetYoutubePlaylistId;

public sealed record GetYoutubePlaylistCommand(string ChannelId, string SongTitle) : IRequest<Result<string>>;