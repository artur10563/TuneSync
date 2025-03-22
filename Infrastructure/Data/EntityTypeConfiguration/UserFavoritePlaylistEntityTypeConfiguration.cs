using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.EntityTypeConfiguration;

public class UserFavoritePlaylistEntityTypeConfiguration : IEntityTypeConfiguration<UserFavoritePlaylist>
{
    public void Configure(EntityTypeBuilder<UserFavoritePlaylist> builder)
    {
        builder.ToTable("UserFavoritePlaylist");
        builder.HasKey(us => new { us.UserGuid, us.PlaylistGuid });
        builder.Property(x => x.IsFavorite).IsRequired().HasDefaultValue(false);
        // builder.Property(x => x.IsOffline).IsRequired().HasDefaultValue(false);
    }
}