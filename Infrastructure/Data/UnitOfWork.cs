using Application.Repositories;
using Application.Repositories.Shared;
using Domain.Entities;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public ISongRepository SongRepository { get; set; }
        public IUserRepository UserRepository { get; set; }
        public IPlaylistRepository PlaylistRepository { get; set; }
        public ILinkEntityRepository<PlaylistSong> PlaylistSongRepository { get; set; }
        public ILinkEntityRepository<UserFavoriteSong> UserFavoriteSongRepository { get; set; }
        public IArtistRepository ArtistRepository { get;set; }
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }



        public UnitOfWork(AppDbContext context,
            ISongRepository songRepository,
            IUserRepository userRepository,
            IPlaylistRepository playlistRepository,
            ILinkEntityRepository<PlaylistSong> playlistSongRepository,
            ILinkEntityRepository<UserFavoriteSong> userFavoriteSongRepository,
            IArtistRepository artistRepository)
        {
            _context = context;
            SongRepository = songRepository ?? throw new ArgumentNullException(nameof(songRepository));
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            PlaylistRepository = playlistRepository ?? throw new ArgumentNullException(nameof(playlistRepository));
            PlaylistSongRepository = playlistSongRepository ?? throw new ArgumentNullException(nameof(playlistSongRepository));
            UserFavoriteSongRepository = userFavoriteSongRepository ?? throw new ArgumentNullException(nameof(userFavoriteSongRepository));
            ArtistRepository = artistRepository ?? throw new ArgumentNullException(nameof(artistRepository));
        }
    }
}
