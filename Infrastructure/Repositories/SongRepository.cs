using Application.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories.Shared;

namespace Infrastructure.Repositories
{
	internal class SongRepository : BaseRepository<Song>, ISongRepository
	{
		public SongRepository(AppDbContext context) : base(context) { }
	}
}
