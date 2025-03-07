using Application.DTOs.Albums;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Album.Query.GetRandomAlbums;

public sealed record GetRandomAlbumsQuery(Guid UserGuid = default) : IRequest<Result<IEnumerable<AlbumSummaryDTO>>>;