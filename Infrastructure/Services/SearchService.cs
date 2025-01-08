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
                join album in _uow.AlbumRepository.Queryable()
                    on song.AlbumGuid equals album.Guid into albumJoin
                from album in albumJoin.DefaultIfEmpty()
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
                let searchQueryVector = EF.Functions.PlainToTsQuery("english", query)

                #endregion

                orderby searchVector.Rank(searchQueryVector) descending
                where searchVector.Matches(searchQueryVector)
                select new { song, artist, album }
            )
            .ToListAsync();

        var songs = sa.Select(x => SongDTO.Create(x.song, false)).ToList();
        return songs;
    }
}