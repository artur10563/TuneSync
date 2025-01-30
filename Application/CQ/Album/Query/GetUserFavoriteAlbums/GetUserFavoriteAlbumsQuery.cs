using Application.DTOs.Playlists;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Album.Query.GetUserFavoriteAlbums;

public sealed record GetUserFavoriteAlbumsQuery(Guid UserGuid = default) : IRequest<Result<List<PlaylistSummaryDTO>>>;