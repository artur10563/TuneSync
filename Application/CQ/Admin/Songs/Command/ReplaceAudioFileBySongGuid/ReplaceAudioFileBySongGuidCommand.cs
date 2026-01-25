using Domain.Primitives;
using MediatR;

namespace Application.CQ.Admin.Songs.Command.ReplaceAudioFileBySongGuid;

public sealed record ReplaceAudioFileBySongGuidCommand(Guid SongGuid, Stream FileStream) : IRequest<Result<Guid>>;