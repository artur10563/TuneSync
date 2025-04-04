﻿using Application.Repositories;
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

        /// <returns>Album with albums(with favoredBy) and songs of artist </returns>
        public async Task<Artist?> GetArtistByGuidAsync(Guid artistGuid)
        {
            return await _set.Include(x => x.Albums)
                .ThenInclude(x => x.FavoredBy)
                .Include(x => x.Songs).AsQueryable()
                .FirstOrDefaultAsync(x => x.Guid == artistGuid);
        }
    }
}