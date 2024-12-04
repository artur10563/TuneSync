using Application.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories.Shared;

namespace Infrastructure.Repositories
{
    public class ArtistRepository : BaseRepository<Artist>, IArtistRepository
    {
        public ArtistRepository(AppDbContext context) : base(context)
        {
        }
    }
}
