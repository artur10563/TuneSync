using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Domain.Primitives;

namespace Infrastructure.Data.EntityTypeConfiguration
{
    internal class SongEntityTypeConfiguration : BaseEntityConfiguration<Song>
    {
        public override void Configure(EntityTypeBuilder<Song> builder)
        {
            base.Configure(builder);

            builder.ToTable("Song");

            builder.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(GlobalVariables.SongConstants.TitleMaxLength);

            builder.Property(s => s.Artist)
                .IsRequired()
                .HasMaxLength(GlobalVariables.SongConstants.TitleMaxLength); // will be replaced with separate table, placeholder for now

            builder.Property(s => s.AudioPath)
                .IsRequired();

            builder.Property(s => s.Source).IsRequired();
            builder.Property(s => s.SourceId).IsRequired(false);
        }
    }
}
