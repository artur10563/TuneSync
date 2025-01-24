using Application.DTOs.Playlists;
using Application.DTOs.Songs;
using Application.Repositories.Shared;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Album.Query.GetAlbumById;

public class GetAlbumByIdQueryHandler : IRequestHandler<GetAlbumByIdQuery, Result<PlaylistDTO>>
{
    private readonly IUnitOfWork _uow;

    public GetAlbumByIdQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<PlaylistDTO>> Handle(GetAlbumByIdQuery request, CancellationToken cancellationToken)
    {
        var userGuid = request.UserGuid ?? Guid.Empty;

        var albumDetails = (await _uow.AlbumRepository.FirstOrDefaultAsync(x => x.Guid == request.AlbumGuid,
            asNoTracking: true,
            p => p.User));

        if (albumDetails == null)
            return Error.NotFound(nameof(Album));

        var albumSongsQuery = _uow.SongRepository
            .Where(s => s.AlbumGuid == request.AlbumGuid,
                asNoTracking: true,
                includes:
                [
                    song => song.Artist,
                    song => song.FavoredBy,
                    song => song.Album
                ])
            .Select(x => SongDTO.Create(x, userGuid));
        
        var totalCount = albumSongsQuery.Count();

        var albumSongs = albumSongsQuery
            .Skip((request.Page - 1) * 25)
            .Take(25)
            .ToList();

        var playlistDto = PlaylistDTO.Create(albumDetails, albumSongs, PageInfo.Create(request.Page, 25, totalCount));

        return playlistDto;
    }
}