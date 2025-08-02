using Application.DTOs;
using Application.DTOs.Songs;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Songs.Query.GetSongsMix;

public record GetSongsMixCommand(
    Guid? UserGuid, 
    List<Guid> AlbumGuids, 
    List<Guid> PlaylistGuids, 
    List<Guid> ArtistGuids,
    int Page = 1, 
    string? ShuffleSeed = null)
    : IPaged, IRequest<PaginatedResult<IEnumerable<SongDTO>>>;