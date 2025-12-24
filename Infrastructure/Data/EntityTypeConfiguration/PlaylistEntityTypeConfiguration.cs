using Domain.Entities;
using Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.EntityTypeConfiguration
{
    internal class PlaylistEntityTypeConfiguration : BaseEntityConfiguration<Playlist>
    {
        public override void Configure(EntityTypeBuilder<Playlist> builder)
        {
            base.Configure(builder);

            builder.ToTable("Playlist");

            builder.Property(x => x.Title).HasMaxLength(GlobalVariables.PlaylistConstants.TitleMaxLength).IsRequired();

            builder.HasOne(x => x.User)
                .WithMany(x => x.Playlists)
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Source).IsRequired();

            builder.Property(x => x.ThumbnailSource).IsRequired(false);
            builder.Property(x => x.ThumbnailId).IsRequired(false);

            // PlaylistSong join entity
            builder.HasMany(p => p.PlaylistSongs)
                .WithOne(ps => ps.Playlist)
                .HasForeignKey(ps => ps.PlaylistGuid);

            builder.Metadata
                .FindNavigation(nameof(Playlist.PlaylistSongs))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}