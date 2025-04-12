using System.Linq.Expressions;
using Application.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AlbumRepository : BaseRepository<Album>, IAlbumRepository
{
    public AlbumRepository(AppDbContext context) : base(context)
    {
    }

    public override Task<Album?> FirstOrDefaultWithDependantAsync(Expression<Func<Album, bool>> predicate)
    {
        var q = _set
            // .Include(album => album.FavoredBy)
            .Include(album => album.Songs)
                .ThenInclude(song => song.FavoredBy)
            .Include(album => album.Songs)
                .ThenInclude(song => song.Playlists); 
        return q.FirstOrDefaultAsync(predicate);
    }
}