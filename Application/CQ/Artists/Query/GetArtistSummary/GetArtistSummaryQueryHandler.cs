using Application.DTOs.Artists;
using Application.DTOs.Songs;
using Application.Repositories.Shared;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Artists.Query.GetArtistSummary;

public class GetArtistSummaryQueryHandler : IRequestHandler<GetArtistSummaryQuery, Result<ArtistSummaryDTO>>
{
    private readonly IUnitOfWork _uow;

    public GetArtistSummaryQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<ArtistSummaryDTO>> Handle(GetArtistSummaryQuery request, CancellationToken cancellationToken)
    {
        var artist = await _uow.ArtistRepository.FirstOrDefaultAsync(x => x.Guid == request.ArtistGuid,
            asNoTracking: true,
            includes: [artist => artist.Albums, artist => artist.Songs]);

        if (artist == null)
            return Error.NotFound(nameof(Artist));

        var currentUserId = request.CurrentUserGuid ?? Guid.Empty;

        var abandonedSongs = artist.Songs
            .Where(x => x.AlbumGuid == null)
            .ToList();


        foreach (var song in abandonedSongs)
        {
            song.Artist = artist;
        }

        var songDTOs = abandonedSongs
            .Select(x => SongDTO.Create(
                x,
                _uow.UserFavoriteSongRepository.Queryable()
                    .Any(usf => usf.SongGuid == x.Guid && usf.UserGuid == currentUserId)
            ))
            .ToList();


        var dto = ArtistSummaryDTO.Create(artist, songDTOs);

        return dto;
    }
}