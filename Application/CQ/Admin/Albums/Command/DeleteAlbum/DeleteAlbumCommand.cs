using Domain.Primitives;
using MediatR;

namespace Application.CQ.Admin.Albums.Command.DeleteAlbum;

public sealed record DeleteAlbumCommand(Guid AlbumGuid) : IRequest<Result>;