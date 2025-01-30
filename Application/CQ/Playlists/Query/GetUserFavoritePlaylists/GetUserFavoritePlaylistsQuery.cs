using Application.DTOs.Playlists;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists.Query.GetUserFavoritePlaylists;

public sealed record GetUserFavoritePlaylistsQuery(Guid UserGuid = default) : IRequest<Result<List<PlaylistSummaryDTO>>>;