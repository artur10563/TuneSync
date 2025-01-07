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
            includes: artist => artist.Albums);

        if (artist == null)
            return Error.NotFound(nameof(Artist));

        var currentUserId = request.CurrentUserGuid ?? Guid.Empty;
        var abandonedSongs =
            (from song in _uow.SongRepository.Queryable()
                join album in _uow.AlbumRepository.Queryable()
                    on song.AlbumGuid equals album.Guid
                join ufs in _uow.UserFavoriteSongRepository.Queryable()
                    on new { guid = song.Guid, userguid = currentUserId } equals new { guid = ufs.SongGuid, userguid = ufs.UserGuid }
                    into leftUfs
                from ufs in leftUfs.DefaultIfEmpty()
                where song.ArtistGuid == artist.Guid
                select new
                {
                    song,
                    IsFavorite = ufs != null
                }).Distinct().ToList();

        var abandonedSongsDTO = abandonedSongs
            .Select(sf =>
            {
                sf.song.Artist = artist;
                return SongDTO.Create(sf.song, sf.IsFavorite);
            });

        var dto = ArtistSummaryDTO.Create(artist, abandonedSongsDTO);

        return dto;
    }
}