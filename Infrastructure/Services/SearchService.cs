using Application.DTOs.Songs;
using Application.Extensions;
using Application.Projections;
using Application.Repositories.Shared;
using Application.Services;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace Infrastructure.Services;

public class SearchService : ISearchService
{
    private readonly IUnitOfWork _uow;
    private readonly IProjectionProvider _projectionProvider;

    public SearchService(IUnitOfWork uow, IProjectionProvider projectionProvider)
    {
        _uow = uow;
        _projectionProvider = projectionProvider;
    }

    public async Task<(List<SongDTO>, int totalItems)> Search(string searchQuery, int page, Guid userGuid = default)
    {

        var query =
        (
            from song in _uow.SongRepository.NoTrackingQueryable()
            
            let searchVector =
                EF.Functions.ToTsVector("english", song.Title ?? " ")
                    .SetWeight(NpgsqlTsVector.Lexeme.Weight.A)
                    .Concat(
                        EF.Functions.ToTsVector("english", song.Artist.DisplayName ?? " ")
                            .SetWeight(NpgsqlTsVector.Lexeme.Weight.B))
                    .Concat(
                        EF.Functions.ToTsVector("english", song.Album.Title ?? " ")
                            .SetWeight(NpgsqlTsVector.Lexeme.Weight.B)
                    )
            let searchQueryVector = EF.Functions.PlainToTsQuery("english", searchQuery)
            
            orderby searchVector.Rank(searchQueryVector) descending
            where searchVector.Matches(searchQueryVector) 
            select song // Whole song wont get selected due to projection
            ).Select(_projectionProvider.GetSongWithArtistProjection(userGuid));
        
        var totalCount = await query.CountAsync();

        var songs = await query.Page(page)
            .Select(x => SongDTO.FromProjection(x))
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