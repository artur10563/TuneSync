using Application.DTOs.Playlists;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Album.Query.GetAlbumById;

public record GetAlbumByIdQuery(Guid AlbumGuid, Guid? UserGuid = null, int Page = 1) : IRequest<Result<PlaylistDTO>>;