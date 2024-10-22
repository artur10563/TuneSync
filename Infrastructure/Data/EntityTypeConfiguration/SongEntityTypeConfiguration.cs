using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
				.HasMaxLength(255); 

			builder.Property(s => s.Artist)
				.IsRequired()
				.HasMaxLength(255);

			builder.Property(s => s.AudioPath)
				.IsRequired();
		}
	}
}
