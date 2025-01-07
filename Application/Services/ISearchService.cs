using Application.DTOs.Songs;

namespace Application.Services;

public interface ISearchService
{
    Task<List<SongDTO>> Search(string query);
}