using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.EntityTypeConfiguration;

public class UserFavoriteAlbumEntityTypeConfiguration : IEntityTypeConfiguration<UserFavoriteAlbum>
{
    public void Configure(EntityTypeBuilder<UserFavoriteAlbum> builder)
    {
        builder.ToTable("UserFavoriteAlbum");
        builder.HasKey(us => new { us.UserGuid, us.AlbumGuid });
        builder.Property(x => x.IsFavorite).IsRequired().HasDefaultValue(false);
        // builder.Property(x => x.IsOffline).IsRequired().HasDefaultValue(false);
    }
}