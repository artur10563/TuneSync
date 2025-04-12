using Domain.Primitives;
using MediatR;

namespace Application.CQ.Admin.Songs.Command.DeleteSong;

public sealed record DeleteSongCommand(Guid SongGuid) : IRequest<Result>;