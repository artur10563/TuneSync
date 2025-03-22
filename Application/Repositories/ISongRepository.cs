using Application.Repositories.Shared;
using Domain.Entities;

namespace Application.Repositories
{
	public interface ISongRepository : IBaseRepository<Song>
	{
		IEnumerable<Song> GetSongWith_Artist_Playlist_Favored();
	}
}
