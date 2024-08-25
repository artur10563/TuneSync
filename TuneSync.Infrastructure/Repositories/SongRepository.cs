using Microsoft.Extensions.Configuration;
using TuneSync.Application.Repositories;
using TuneSync.Domain.Entities;

namespace TuneSync.Infrastructure.Repositories
{
	public class SongRepository : BaseRepository<Song>, ISongRepository
	{
		public SongRepository(IConfiguration configuration) : base(configuration, nameof(Song))
		{
		}
	}
}
