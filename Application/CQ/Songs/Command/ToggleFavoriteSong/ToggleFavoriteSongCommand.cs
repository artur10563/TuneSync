using Domain.Primitives;
using MediatR;

namespace Application.CQ.Songs.Command;

public sealed record ToggleFavoriteSongCommand(Guid SongGuid, Guid UserGuid) : IRequest<Result>;