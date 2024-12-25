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
            includes: artist => artist.Playlists);

        if (artist == null)
            return Error.NotFound(nameof(Artist));

        var currentUserId = request.CurrentUserGuid ?? Guid.Empty;
        var abandonedSongs =
            (from song in _uow.SongRepository.Queryable()
                join ps in _uow.PlaylistSongRepository.Queryable()
                    on song.Guid equals ps.SongGuid into leftPs
                from ps in leftPs.DefaultIfEmpty()
                join playlist in _uow.PlaylistRepository.Queryable()
                    on ps.PlaylistGuid equals playlist.Guid into leftPlaylist
                from playlist in leftPlaylist.DefaultIfEmpty()
                join ufs in _uow.UserFavoriteSongRepository.Queryable()
                    on new { guid = song.Guid, userguid = currentUserId } equals new { guid = ufs.SongGuid, userguid = ufs.UserGuid }
                    into leftUfs
                from ufs in leftUfs.DefaultIfEmpty()
                where song.ArtistGuid == artist.Guid && playlist.Source != "Youtube"
                select new
                {
                    song,
                    IsFavorite = ufs != null
                }).ToList();
        
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