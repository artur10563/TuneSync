using Application.Repositories;
using Application.Repositories.Shared;
using Domain.Entities;
using Domain.Entities.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public ISongRepository SongRepository { get; set; }
        public IUserRepository UserRepository { get; set; }
        public IPlaylistRepository PlaylistRepository { get; set; }
        public IAlbumRepository AlbumRepository { get; set; }
        public ILinkEntityRepository<PlaylistSong> PlaylistSongRepository { get; set; }
        public ILinkEntityRepository<UserSong> UserSongRepository { get; set; }
        public ILinkEntityRepository<UserFavoriteAlbum> UserFavoriteAlbumRepository { get; set; }
        public ILinkEntityRepository<UserFavoritePlaylist> UserFavoritePlaylistRepository { get; }
        public IArtistRepository ArtistRepository { get; set; }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void SetSeed(double seed)
        {
            
            if (seed is < 0 or > 1) throw new Exception("Seed must be between 0 and 1");
            _context.Database.ExecuteSql($"SELECT setseed({seed})");
        }

        public IQueryable<T> ApplyOrdering<T>(IQueryable<T> query, string orderBy, bool isDescending) where T : class
        {
            return string.IsNullOrEmpty(orderBy) 
                ? query 
                : isDescending 
                    ? query.OrderByDescending(entity => EF.Property<object>(entity, orderBy)) 
                    : query.OrderBy(entity => EF.Property<object>(entity, orderBy));
        }

        public void TransactedAction(Action action)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                action.Invoke();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw new Exception("Transaction failed");
            }
        }
        
        public async Task TransactedActionAsync(Func<Task> action)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await action.Invoke();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new Exception("Transaction failed");
            }
        }

        public UnitOfWork(AppDbContext context,
            ISongRepository songRepository,
            IUserRepository userRepository,
            IPlaylistRepository playlistRepository,
            ILinkEntityRepository<PlaylistSong> playlistSongRepository,
            ILinkEntityRepository<UserSong> userSongRepository,
            ILinkEntityRepository<UserFavoriteAlbum> userFavoriteAlbumRepository,
            ILinkEntityRepository<UserFavoritePlaylist> userFavoritePlaylistRepository,
            IArtistRepository artistRepository,
            IAlbumRepository albumRepository)
        {
            _context = context;
            SongRepository = songRepository ?? throw new ArgumentNullException(nameof(songRepository));
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            PlaylistRepository = playlistRepository ?? throw new ArgumentNullException(nameof(playlistRepository));
            PlaylistSongRepository = playlistSongRepository ?? throw new ArgumentNullException(nameof(playlistSongRepository));
            UserSongRepository = userSongRepository ?? throw new ArgumentNullException(nameof(userSongRepository));
            UserFavoriteAlbumRepository = userFavoriteAlbumRepository ?? throw new ArgumentNullException(nameof(userFavoriteAlbumRepository));
            UserFavoritePlaylistRepository = userFavoritePlaylistRepository ?? throw new ArgumentNullException(nameof(userFavoritePlaylistRepository));
            ArtistRepository = artistRepository ?? throw new ArgumentNullException(nameof(artistRepository));
            AlbumRepository = albumRepository ?? throw new ArgumentNullException(nameof(albumRepository));
        }
    }
}