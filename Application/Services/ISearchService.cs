using Application.DTOs.Songs;

namespace Application.Services;

public interface ISearchService
{
    Task<(List<SongDTO>, int totalItems)> Search(string searchQuery, int page, Guid userGuid  = default);
    
    /// <summary>
    /// Shuffles query based on provided seed
    /// </summary>
    /// <param name="query">Query to shuffle</param>
    /// <param name="shuffleSeed">Seed which is used for shuffle. Range: 0...1</param>
    /// <returns>Shuffled query</returns>
    IQueryable<T> Shuffle<T>(IQueryable<T> query, double? shuffleSeed = null);
}