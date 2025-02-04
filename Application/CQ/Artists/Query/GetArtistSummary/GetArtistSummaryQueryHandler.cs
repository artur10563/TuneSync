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
        _logger.Log("Fetching artist summary", LogLevel.Debug, request);
        try
        {
            var artist = await _uow.ArtistRepository.FirstOrDefaultAsync(x => x.Guid == request.ArtistGuid);

            if (artist == null)
                return Error.NotFound(nameof(Artist));

            var abandonedSongs = _uow.SongRepository
                .Where(x => x.ArtistGuid == artist.Guid && x.AlbumGuid == null)
                .ToList();

            foreach (var song in abandonedSongs)
            {
                song.Artist = artist;
            }

            var userFavoriteSongs = new HashSet<Guid>(_uow.UserSongRepository
                .Where(usf => usf.UserGuid == request.CurrentUserGuid && usf.IsFavorite)
                .Select(x => x.SongGuid)
                .ToList());

            var userFavoriteAlbums = new HashSet<Guid>(_uow.UserFavoriteAlbumRepository
                .Where(ufa => ufa.UserGuid == request.CurrentUserGuid && ufa.IsFavorite)
                .Select(x => x.AlbumGuid)
                .ToList());

            var songDTOs = abandonedSongs
                .Select(x => SongDTO.Create(
                    x,
                    userFavoriteSongs.Any(usfGuid => usfGuid == x.Guid)
                )).ToList();

            var albumSongCount = _uow.AlbumRepository
                .Where(album => album.ArtistGuid == artist.Guid)
                .Select(album => new
                {
                    album,
                    songCount = _uow.SongRepository.Where(song => song.AlbumGuid == album.Guid).Count()
                })
                .ToList();


            var albumDTOs = albumSongCount.Select(albumInfo => AlbumSummaryDTO.Create(
                    albumInfo.album,
                    artist,
                    isFavorite: userFavoriteAlbums.Contains(albumInfo.album.Guid),
                    songCount: albumInfo.songCount
                )
            ).ToList();
            return new ArtistSummaryDTO(ArtistInfoDTO.Create(artist), albumDTOs, songDTOs);
        }
        catch (Exception ex)
        {
            _logger.Log(ex.Message, LogLevel.Error, request, ex);
        }

        return Error.None;
    }
}