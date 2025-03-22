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
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Property(x => x.Source).IsRequired();

            builder.Property(x=>x.ThumbnailSource).IsRequired(false);
            builder.Property(x=>x.ThumbnailId).IsRequired(false);
            
            builder.HasMany(p => p.Songs)
                .WithMany(s => s.Playlists)
                .UsingEntity<PlaylistSong>(
                pls =>
                    pls.HasOne<Song>()
                    .WithMany()
                    .HasForeignKey(pls => pls.SongGuid),
                pls =>
                    pls.HasOne<Playlist>()
                    .WithMany()
                    .HasForeignKey(pls => pls.PlaylistGuid),
                j =>
                {
                    j.ToTable("PlaylistSong");
                    j.HasKey(ps => new { ps.PlaylistGuid, ps.SongGuid });
                    j.HasIndex(ps => new { ps.PlaylistGuid, ps.SongGuid }).IsUnique();

                    j.HasOne(ps => ps.Playlist)
                        .WithMany()
                        .HasForeignKey(ps => ps.PlaylistGuid)
                        .OnDelete(DeleteBehavior.Cascade);
                    
                    j.HasOne(ps => ps.Song)
                        .WithMany()
                        .HasForeignKey(ps => ps.SongGuid)
                        .OnDelete(DeleteBehavior.Cascade);

                });
        }
    }
}
