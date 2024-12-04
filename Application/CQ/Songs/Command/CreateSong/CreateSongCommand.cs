using Domain.Entities;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Songs.Command.CreateSong
{
	public sealed record CreateSongCommand(string Title, Guid artistGuid, Stream audioFileStream) : IRequest<Result<Song>>;
}
