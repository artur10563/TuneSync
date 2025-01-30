using Domain.Entities;
using Domain.Entities.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Song> Songs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Album> Album { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
        public DbSet<UserSong> UserSongs { get; set; }
        public DbSet<UserFavoriteAlbum> UserFavoriteAlbums { get; set; }
        public DbSet<UserFavoritePlaylist> UserFavoritePlaylists { get; set; }
        public DbSet<Artist> Artists { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(builder);
        }

    }
}
