using Application.DTOs;
using Application.DTOs.Songs;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Album.Query.GetAlbumSongsById;

public record GetAlbumSongsByIdQuery(Guid AlbumGuid, Guid? UserGuid = null, int Page = 1) 
    : IPaged, IRequest<PaginatedResult<IEnumerable<SongDTO>>>;