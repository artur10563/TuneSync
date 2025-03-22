using Domain.Primitives;
using MediatR;

namespace Application.CQ.Album.Command;

public sealed record ToggleFavoriteAlbumCommand(Guid AlbumGuid, Guid UserGuid) : IRequest<Result>;