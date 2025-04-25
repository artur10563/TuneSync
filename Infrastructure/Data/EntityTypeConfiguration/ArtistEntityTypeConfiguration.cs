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
            builder.Property(x => x.ThumbnailUrl).IsRequired(false).HasMaxLength(255);
            builder.Property(x => x.YoutubeChannelId).IsRequired();
            builder.HasIndex(x => x.YoutubeChannelId).IsUnique();

            builder.HasMany(x => x.Songs)
                .WithOne(x => x.Artist)
                .HasForeignKey(x => x.ArtistGuid)
                .OnDelete(DeleteBehavior.Cascade);
            
            
            //TODO: re-test if deletion of Parent/TopParent works with nested children
            // On deletion need to manually re-calculate top lvl parentId
            builder.HasMany(x => x.Children)
                .WithOne(x => x.Parent)
                .HasForeignKey(x => x.ParentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.AllChildren)
                .WithOne(x => x.TopLvlParent)
                .HasForeignKey(x => x.TopLvlParentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
