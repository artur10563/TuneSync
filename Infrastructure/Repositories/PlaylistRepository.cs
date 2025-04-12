using System.Linq.Expressions;
using Application.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PlaylistRepository : BaseRepository<Playlist>, IPlaylistRepository
    {
        public PlaylistRepository(AppDbContext context) : base(context)
        {
        }

        public override Task<Playlist?> FirstOrDefaultWithDependantAsync(Expression<Func<Playlist, bool>> predicate)
        {
            return _set.Include(pl => pl.Songs).FirstOrDefaultAsync();
        }
    }
}
