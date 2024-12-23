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
                where song.ArtistGuid == artist.Guid
                      && (ps == null) // Ensure the song is not in any playlist
                      && (playlist == null || playlist.Source == "Youtube")
                select song).ToList();

        
        var favoriteSongGuids = _uow.UserFavoriteSongRepository
            .Where(f => f.UserGuid == currentUserId)
            .Select(f => f.SongGuid)
            .ToHashSet();
        
        var abandonedSongsDTO = abandonedSongs
            .Select(song =>
            {
                var isFavorite = favoriteSongGuids.Contains(song.Guid);

                song.Artist = artist;
                return SongDTO.Create(song, isFavorite);
            });

        var dto = ArtistSummaryDTO.Create(artist, abandonedSongsDTO);

        return dto;
    }
}