using Application.Repositories.Shared;
using Domain.Entities;

namespace Application.Repositories
{
    public interface IArtistRepository : IBaseRepository<Artist>
    {
        /// <returns>Album with albums(with favoredBy) and songs of artist </returns>
        Task<Artist?> GetArtistByGuidAsync(Guid artistGuid);
    }
}
