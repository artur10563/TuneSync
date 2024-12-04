using Domain.Entities;
using Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.EntityTypeConfiguration
{
    internal class ArtistEntityTypeConfiguration : BaseEntityConfiguration<Artist>
    {
        public override void Configure(EntityTypeBuilder<Artist> builder)
        {
            base.Configure(builder);

            builder.ToTable(nameof(Artist));

            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.DisplayName)
                .HasMaxLength(GlobalVariables.UserConstants.NameMaxLength)
                .IsRequired();
            builder.Property(x => x.YoutubeChannelId).IsRequired();

            builder.HasIndex(x => x.YoutubeChannelId).IsUnique();

            builder.HasMany(x => x.Songs)
                .WithOne(x => x.Artist)
                .HasForeignKey(x => x.ArtistGuid)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
