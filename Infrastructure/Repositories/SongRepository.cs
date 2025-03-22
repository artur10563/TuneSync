using Application.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
	internal class SongRepository : BaseRepository<Song>, ISongRepository
	{
		public SongRepository(AppDbContext context) : base(context) { }

		public IEnumerable<Song> GetSongWith_Artist_Playlist_Favored()
		{
			return _set
				.Include(s => s.Artist)
				.Include(s => s.Playlists)
				.Include(s => s.FavoredBy);
		}
	}
}
