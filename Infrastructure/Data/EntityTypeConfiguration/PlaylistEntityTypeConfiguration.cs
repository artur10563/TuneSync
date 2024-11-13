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
        }
    }
}
