using Application.DTOs.Songs;
using Application.Extensions;
using Application.Repositories.Shared;
using Application.Services;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace Infrastructure.Services;

public class SearchService : ISearchService
{
    private readonly IUnitOfWork _uow;
    private readonly ISearchService _searchService;

    public SearchService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<(List<SongDTO>, int totalItems)> Search(string searchQuery, int page, Guid userGuid = default)
    {
        var query = (
            from song in _uow.SongRepository.NoTrackingQueryable()
            join artist in _uow.ArtistRepository.NoTrackingQueryable()
                on song.ArtistGuid equals artist.Guid
            join album in _uow.AlbumRepository.NoTrackingQueryable()
                on song.AlbumGuid equals album.Guid into albumJoin
            from album in albumJoin.DefaultIfEmpty()
            join us in _uow.UserSongRepository.NoTrackingQueryable()
                on new { songGuid = song.Guid, userGuid = userGuid } equals new { songGuid = us.SongGuid, userGuid = us.UserGuid } into userSongJoin
            from userSong in userSongJoin.DefaultIfEmpty()

            #region vector

            let searchVector =
                EF.Functions.ToTsVector("english", song.Title ?? " ")
                    .SetWeight(NpgsqlTsVector.Lexeme.Weight.A)
                    .Concat(
                        EF.Functions.ToTsVector("english", artist.DisplayName ?? " ")
                            .SetWeight(NpgsqlTsVector.Lexeme.Weight.B))
                    .Concat(
                        EF.Functions.ToTsVector("english", album.Title ?? " ")
                            .SetWeight(NpgsqlTsVector.Lexeme.Weight.B)
                    )
            let searchQueryVector = EF.Functions.PlainToTsQuery("english", searchQuery)

            #endregion

            orderby searchVector.Rank(searchQueryVector) descending
            where searchVector.Matches(searchQueryVector)
            select new { song, artist, album, isFavorite = userSong != null && userSong.IsFavorite }
        );

        var totalCount = await query.CountAsync();

        var songs = await query.Page(page)
            .Select(x => SongDTO.Create(x.song, x.artist, x.isFavorite))
            .ToListAsync();

        return (songs, totalCount);
    }

    //Will work only in transacted action due to shuffle seed
    public IQueryable<T> Shuffle<T>(IQueryable<T> query, double? shuffleSeed = null)
    {
        if (shuffleSeed is >= 0 and <= 1)
            _uow.SetSeed(shuffleSeed.Value);
        return query.OrderBy(x => EF.Functions.Random());
    }
}