using Application.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ArtistRepository : BaseRepository<Artist>, IArtistRepository
    {
        public ArtistRepository(AppDbContext context) : base(context)
        {
        }
        
        public IQueryable<Artist> WithAlbumAndSongs()
        {
            return _set.Include(x => x.Albums)
                .ThenInclude(x => x.FavoredBy)
                .Include(x => x.Songs).AsQueryable();
        }
    }
}
