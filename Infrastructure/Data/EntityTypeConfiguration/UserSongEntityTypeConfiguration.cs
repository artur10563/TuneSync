using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.EntityTypeConfiguration;

internal class UserSongEntityTypeConfiguration : IEntityTypeConfiguration<UserSong>
{
    public void Configure(EntityTypeBuilder<UserSong> builder)
    {
        builder.ToTable("UserSongs");

        builder.HasKey(us => new { us.UserGuid, us.SongGuid });

        builder.HasOne(us => us.User)
            .WithMany(u => u.FavoriteSongs)
            .HasForeignKey(us => us.UserGuid)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(us => us.Song)
            .WithMany(s => s.FavoredBy)
            .HasForeignKey(us => us.SongGuid)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(us => us.IsFavorite)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(us => us.IsOffline)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(us => us.CreatedAt)
            .HasDefaultValueSql("NOW()");
       
    }
}