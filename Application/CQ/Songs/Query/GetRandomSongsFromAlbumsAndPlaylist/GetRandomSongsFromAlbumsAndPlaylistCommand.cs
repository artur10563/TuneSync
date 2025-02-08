using Application.DTOs.Songs;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Songs.Query.GetRandomSongsFromAlbumsAndPlaylist;

public record GetRandomSongsFromAlbumsAndPlaylistCommand(Guid? UserGuid, List<Guid> AlbumGuids, List<Guid> PlaylistGuids, int page = 1, string? ShuffleSeed = null)
    : IRequest<PaginatedResult<IEnumerable<SongDTO>>>;