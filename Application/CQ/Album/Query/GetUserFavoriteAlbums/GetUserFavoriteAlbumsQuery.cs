using Application.DTOs.Albums;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Album.Query.GetUserFavoriteAlbums;

public sealed record GetUserFavoriteAlbumsQuery(Guid UserGuid = default) : IRequest<Result<List<AlbumSummaryDTO>>>;