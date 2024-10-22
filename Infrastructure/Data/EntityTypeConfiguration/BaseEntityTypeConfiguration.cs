using Domain.Entities.Shared;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.EntityTypeConfiguration
{
	internal abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : EntityBase
	{
		public virtual void Configure(EntityTypeBuilder<TEntity> builder)
		{
			builder.HasKey(e => e.Guid);
			builder.Property(e => e.CreatedAt)
				.HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
				.ValueGeneratedOnAdd();

			builder.Property(e => e.ModifiedAt)
				.HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
				.ValueGeneratedOnUpdate();
		}
	}
}
