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

    public async Task<List<SongDTO>> Search(string query)
    {
        var sa = await (
                from song in _uow.SongRepository.Queryable()
                join artist in _uow.ArtistRepository.Queryable()
                    on song.ArtistGuid equals artist.Guid
                join pls in _uow.PlaylistSongRepository.Queryable()
                    on song.Guid equals pls.SongGuid into pls
                from plsd in pls.DefaultIfEmpty()
                join playlist in _uow.PlaylistRepository.Queryable()
                    on plsd.PlaylistGuid equals playlist.Guid
        
                #region vector
        
                let searchVector =
                    EF.Functions.ToTsVector("english", song.Title ?? " ")
                        .SetWeight(NpgsqlTsVector.Lexeme.Weight.A).Concat(
                            EF.Functions.ToTsVector("english", artist.DisplayName ?? " ")
                                .SetWeight(NpgsqlTsVector.Lexeme.Weight.B))
                let searchQueryVector = EF.Functions.PlainToTsQuery("english", query)
        
                #endregion
        
                orderby searchVector.Rank(searchQueryVector) descending
                where searchVector.Matches(searchQueryVector)
                select new { song, artist, playlist }
            )
            .ToListAsync();
        
        var songs = sa.Select(x => SongDTO.Create(x.song, false)).ToList();
        return songs;
    }
}