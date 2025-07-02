using Application.DTOs.Albums;
using Application.DTOs.Songs;
using Application.Extensions;
using Application.Projections;
using Application.Repositories.Shared;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Album.Query.GetAlbumById;

public class GetAlbumByIdQueryHandler : IRequestHandler<GetAlbumByIdQuery, Result<AlbumDTO>>
{
    private readonly IUnitOfWork _uow;
    private readonly IProjectionProvider _projectionProvider;

    public GetAlbumByIdQueryHandler(IUnitOfWork uow, IProjectionProvider projectionProvider)
    {
        _uow = uow;
        _projectionProvider = projectionProvider;
    }

    public async Task<Result<AlbumDTO>> Handle(GetAlbumByIdQuery request, CancellationToken cancellationToken)
    {
        var pageSize = 25;
        var userGuid = request.UserGuid ?? Guid.Empty;

        var albumInfo = _uow.AlbumRepository
            .NoTrackingQueryable()
            .Where(x => x.Guid == request.AlbumGuid)
            .Select(_projectionProvider.GetAlbumWithArtistProjection(userGuid))
            .FirstOrDefault();
        
        if (albumInfo == null)
            return Error.NotFound(nameof(Album));


        var albumSongsQuery = _uow.SongRepository
            .NoTrackingQueryable()
            .Where(s => s.AlbumGuid == request.AlbumGuid)
            .OrderBy(x => x.CreatedAt)
            .Select(_projectionProvider.GetSongWithArtistProjection(userGuid))
            .Page(request.Page, pageSize)
            .ToList();

        var songDtos = albumSongsQuery.Select(SongDTO.FromProjection).ToList();

        var pageInfo = PageInfo.Create(request.Page, pageSize, albumInfo.SongCount);
        var songPage = new PaginatedResponse<ICollection<SongDTO>>(songDtos, pageInfo);

        var albumDto = AlbumDTO.FromProjection(albumInfo, songPage); 

        
        return albumDto;
    }
}