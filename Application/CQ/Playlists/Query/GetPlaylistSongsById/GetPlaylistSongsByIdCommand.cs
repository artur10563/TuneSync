using Application.DTOs.Songs;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Playlists.Query.GetPlaylistSongsById;

public sealed record GetPlaylistSongsByIdCommand(Guid UserGuid, Guid PlaylistGuid, int Page) : IRequest<PaginatedResult<IEnumerable<SongDTO>>>;