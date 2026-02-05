using Application.DTOs.Songs;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Songs.Query.GetSongByGuid;

public sealed record GetSongByGuidCommand(Guid SongGuid, Guid? UserGuid) : IRequest<Result<SongDTO>>;