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
        builder.Property(x => x.IsFavorite).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.IsOffline).IsRequired().HasDefaultValue(false);
    }
}