using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.EntityTypeConfiguration;

internal class PlaylistSongEntityTypeConfiguration : IEntityTypeConfiguration<PlaylistSong>
{
    public void Configure(EntityTypeBuilder<PlaylistSong> builder)
    {
        builder.ToTable("PlaylistSong");

        builder.HasKey(ps => new { ps.PlaylistGuid, ps.SongGuid });

        builder.HasOne(ps => ps.Playlist)
            .WithMany(p => p.PlaylistSongs)
            .HasForeignKey(ps => ps.PlaylistGuid)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ps => ps.Song)
            .WithMany()
            .HasForeignKey(ps => ps.SongGuid)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(ps => ps.CreatedAt)
            .HasDefaultValueSql("NOW()");
    }
}