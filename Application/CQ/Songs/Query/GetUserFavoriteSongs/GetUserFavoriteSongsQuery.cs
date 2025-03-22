using Application.DTOs.Songs;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Songs.Query.GetUserFavoriteSongs;

public sealed record GetUserFavoriteSongsQuery(Guid UserGuid) : IRequest<Result<IEnumerable<SongDTO>>>;