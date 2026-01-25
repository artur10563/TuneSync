using System.Linq.Expressions;
using Application.Repositories.Shared;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

            builder.Entity<Song>().HasQueryFilter(CommonFilter.HasAudioFilter, x => x.AudioPath != null);

            base.OnModelCreating(builder);
        }
    }

    public static class QueryableFilterExtensions
    {
        public static EntityTypeBuilder<TEntity> HasQueryFilter<TEntity>(
            this EntityTypeBuilder<TEntity> builder, CommonFilter filterKey, Expression<Func<TEntity, bool>> filter
        ) where TEntity : class
        {
            return builder.HasQueryFilter(filterKey.ToString(), (LambdaExpression)filter);
        }
    }
}