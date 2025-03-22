using Domain.Entities;
using Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.EntityTypeConfiguration;

internal class AlbumEntityTypeConfiguration : BaseEntityConfiguration<Album>
{
    public override void Configure(EntityTypeBuilder<Album> builder)
    {
        base.Configure(builder);

        builder.ToTable(nameof(Album));

        builder.Property(x => x.Title).HasMaxLength(GlobalVariables.PlaylistConstants.TitleMaxLength).IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Albums)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.SourceId).IsRequired(true);

        builder.Property(x => x.ThumbnailSource).IsRequired(false);
        builder.Property(x => x.ThumbnailId).IsRequired(false);

        builder.HasMany(a => a.Songs)
            .WithOne(s => s.Album)
            .HasForeignKey(s => s.AlbumGuid)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(a => a.Artist)
            .WithMany(a => a.Albums)
            .HasForeignKey(a => a.ArtistGuid)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(a => a.ExpectedSongs)
            .IsRequired(true);
    }
}