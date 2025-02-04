using Application.DTOs.Songs;
using Application.Repositories.Shared;
using Application.Services;
using Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace Infrastructure.Services;

public class SearchService : ISearchService
{
    private readonly IUnitOfWork _uow;

    public SearchService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<(List<SongDTO>, int totalItems)> Search(string searchQuery, int page, Guid userGuid = default)
    {
        page = page <= 0 ? 1 : page;
        var pageSize = 25;
        
        var query = (
            from song in _uow.SongRepository.NoTrackingQueryable()
            join artist in _uow.ArtistRepository.NoTrackingQueryable()
                on song.ArtistGuid equals artist.Guid
            join album in _uow.AlbumRepository.NoTrackingQueryable()
                on song.AlbumGuid equals album.Guid into albumJoin
            from album in albumJoin.DefaultIfEmpty()
            
            join us in _uow.UserSongRepository.NoTrackingQueryable()
                on new {songGuid = song.Guid, userGuid =  userGuid} equals new {songGuid = us.SongGuid, userGuid = us.UserGuid} into userSongJoin
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

        var sa = await query.Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var songs = sa.Select(x => SongDTO.Create(x.song, x.isFavorite)).ToList();
        return (songs, totalCount);
    }
}