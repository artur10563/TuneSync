using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.EntityTypeConfiguration
{
    internal class UserEntityTypeConfiguration : BaseEntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.HasIndex(x => x.IdentityId).IsUnique();
            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.Email).HasMaxLength(255).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();

        }
    }
}
