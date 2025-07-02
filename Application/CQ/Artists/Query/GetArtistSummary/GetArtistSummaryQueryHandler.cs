using Application.DTOs.Albums;
using Application.DTOs.Artists;
using Application.DTOs.Songs;
using Application.Projections;
using Application.Repositories.Shared;
using Application.Services;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Artists.Query.GetArtistSummary;

public class GetArtistSummaryQueryHandler : IRequestHandler<GetArtistSummaryQuery, Result<ArtistSummaryDTO>>
{
    private readonly IUnitOfWork _uow;
    private readonly ILoggerService _logger;
    private readonly IProjectionProvider _projectionProvider;

    public GetArtistSummaryQueryHandler(IUnitOfWork uow, ILoggerService logger, IProjectionProvider projectionProvider)
    {
        _uow = uow;
        _logger = logger;
        _projectionProvider = projectionProvider;
    }

    public async Task<Result<ArtistSummaryDTO>> Handle(GetArtistSummaryQuery request, CancellationToken cancellationToken)
    {
        var exists = await _uow.ArtistRepository.ExistsAsync(x => x.Guid == request.ArtistGuid);
        if(!exists) return Error.NotFound(nameof(Artist));
        
        //Get songs of artist
        var standaloneSongs = _uow.SongRepository.NoTrackingQueryable()
            .Where(x => x.Artist.TopLvlParent.Guid == request.ArtistGuid || x.Guid == request.ArtistGuid)
            .Where(x => x.AlbumGuid == null)
            .Select(_projectionProvider.GetSongWithArtistProjection(request.CurrentUserGuid))
            .Select(x => SongDTO.FromProjection(x))
            .ToList();
        
        //Get albums summary (albums without songs)
        var albums = _uow.AlbumRepository
            .NoTrackingQueryable()
            .Where(x => x.ArtistGuid == request.ArtistGuid || x.Artist.TopLvlParent.Guid == request.ArtistGuid)
            .Select(_projectionProvider.GetAlbumSummaryProjection(request.CurrentUserGuid))
            .Select(x => AlbumSummaryDTO.FromProjection(x))
            .ToList();

        var resultDto = new ArtistSummaryDTO(albums.First().Artist!, albums, standaloneSongs);
        return resultDto;
    }
}