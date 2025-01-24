using Application.DTOs.Songs;

namespace Application.Services;

public interface ISearchService
{
    Task<(List<SongDTO>, int totalItems)> Search(string searchQuery, int page, Guid userGuid  = default);
}