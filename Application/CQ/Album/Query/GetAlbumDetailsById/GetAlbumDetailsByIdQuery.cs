using Application.DTOs.Albums;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Album.Query.GetAlbumDetailsById;

public record GetAlbumDetailsByIdQuery(Guid AlbumGuid, Guid? UserGuid = null) : IRequest<Result<AlbumDTO>>;