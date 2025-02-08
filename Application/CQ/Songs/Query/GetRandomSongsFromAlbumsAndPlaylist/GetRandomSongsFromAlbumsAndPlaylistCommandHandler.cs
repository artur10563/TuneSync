using Application.DTOs.Songs;
using Application.Extensions;
using Application.Repositories.Shared;
using Application.Services;
using Domain.Primitives;
using FluentValidation;
using MediatR;

namespace Application.CQ.Songs.Query.GetRandomSongsFromAlbumsAndPlaylist;

public class GetRandomSongsFromAlbumsAndPlaylistCommandHandler : IRequestHandler<GetRandomSongsFromAlbumsAndPlaylistCommand, PaginatedResult<IEnumerable<SongDTO>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ISearchService _searchService;
    private readonly IValidator<GetRandomSongsFromAlbumsAndPlaylistCommand> _validator;

    public GetRandomSongsFromAlbumsAndPlaylistCommandHandler(
        IUnitOfWork uow,
        ISearchService searchService,
        IValidator<GetRandomSongsFromAlbumsAndPlaylistCommand> validator)
    {
        _uow = uow;
        _searchService = searchService;
        _validator = validator;
    }

    private double GetRandomDoubleFromSeed(string? seed = null)
    {
        seed = seed?.Trim();
        seed = string.IsNullOrEmpty(seed) ? Guid.NewGuid().ToString() : seed;
        return new Random(seed.GetHashCode()).NextDouble();
    }

    public async Task<PaginatedResult<IEnumerable<SongDTO>>> Handle(GetRandomSongsFromAlbumsAndPlaylistCommand request, CancellationToken cancellationToken)
    {
        var validationErrors = await _validator.ValidateAsync(request);
        if (!validationErrors.IsValid)
            return validationErrors.AsErrors(request.page);

        var userGuid = request.UserGuid ?? Guid.Empty;

        var baseQuery = (from song in _uow.SongRepository.NoTrackingQueryable()
                join songPlaylist in _uow.PlaylistSongRepository.NoTrackingQueryable()
                    on song.Guid equals songPlaylist.SongGuid into songPlaylistsJoin
                from songPlaylist in songPlaylistsJoin.DefaultIfEmpty()
                where
                    (request.AlbumGuids.Count != 0 && song.AlbumGuid.HasValue && request.AlbumGuids.Contains(song.AlbumGuid.Value)) ||
                    (request.PlaylistGuids.Count != 0 && request.PlaylistGuids.Contains(songPlaylist.PlaylistGuid))
                select song
            );

        var songQuery = (
            from song in baseQuery
            join artist in _uow.ArtistRepository.NoTrackingQueryable()
                on song.ArtistGuid equals artist.Guid into artistsJoin
            from artist in artistsJoin.DefaultIfEmpty()
            join album in _uow.AlbumRepository.NoTrackingQueryable()
                on song.AlbumGuid equals album.Guid into albumsJoin
            from album in albumsJoin.DefaultIfEmpty()
            join us in _uow.UserSongRepository.NoTrackingQueryable()
                on new { songGuid = song.Guid, userGuid = userGuid } equals new { songGuid = us.SongGuid, userGuid = us.UserGuid } into userSongJoin
            from userSong in userSongJoin.DefaultIfEmpty()
            select SongDTO.Create(song, album, artist, userSong != null && userSong.IsFavorite)
        );
        
        var songs = new List<SongDTO>();
        
        //Transaction is required for ShuffleSeed to work 
        _uow.TransactedAction(() =>
        {
            songs = _searchService.Shuffle(songQuery, GetRandomDoubleFromSeed(request.ShuffleSeed))
                .Page(request.page)
                .ToList();
        });

        return (songs, request.page, baseQuery.Count());
    }
}