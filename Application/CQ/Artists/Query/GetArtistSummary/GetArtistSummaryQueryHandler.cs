using Application.DTOs.Albums;
using Application.DTOs.Artists;
using Application.DTOs.Songs;
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

    public GetArtistSummaryQueryHandler(IUnitOfWork uow, ILoggerService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<Result<ArtistSummaryDTO>> Handle(GetArtistSummaryQuery request, CancellationToken cancellationToken)
    {
        var artist = await _uow.ArtistRepository.FirstOrDefaultAsync(x => x.Guid == request.ArtistGuid, 
            asNoTracking: true,
            includes: x => x.AllChildren);

        if (artist == null)
            return Error.NotFound(nameof(Artist));

        var artistGuids = new HashSet<Guid> { artist.Guid };
        artistGuids.UnionWith(artist.AllChildren.Select(child => child.Guid));

        
        var abandonedSongs = _uow.SongRepository
            .Where(x => artistGuids.Contains(x.ArtistGuid) && x.AlbumGuid == null, asNoTracking: true)
            .ToList();

        foreach (var song in abandonedSongs)
        {
            song.Artist = artist;
        }

        var userFavoriteSongs = new HashSet<Guid>(_uow.UserSongRepository
            .Where(usf => usf.UserGuid == request.CurrentUserGuid && usf.IsFavorite, asNoTracking: true)
            .Select(x => x.SongGuid)
            .ToList());

        var userFavoriteAlbums = new HashSet<Guid>(_uow.UserFavoriteAlbumRepository
            .Where(ufa => ufa.UserGuid == request.CurrentUserGuid && ufa.IsFavorite, asNoTracking: true)
            .Select(x => x.AlbumGuid)
            .ToList());

        var songDTOs = abandonedSongs
            .Select(x => SongDTO.Create(
                x,
                artist,
                userFavoriteSongs.Any(usfGuid => usfGuid == x.Guid)
            )).ToList();

        var albumSongCount =
            (from album in _uow.AlbumRepository.NoTrackingQueryable()
                join song in _uow.SongRepository.NoTrackingQueryable()
                    on album.Guid equals song.AlbumGuid into songs
                where artistGuids.Contains(album.ArtistGuid.Value)
                select new
                {
                    album = album,
                    songCount = songs.Count()
                }).ToList();


        var albumDTOs = albumSongCount.Select(albumInfo => AlbumSummaryDTO.Create(
                albumInfo.album,
                artist,
                isFavorite: userFavoriteAlbums.Contains(albumInfo.album.Guid),
                songCount: albumInfo.songCount
            )
        ).ToList();
        return new ArtistSummaryDTO(ArtistInfoDTO.Create(artist), albumDTOs, songDTOs);

    }
}