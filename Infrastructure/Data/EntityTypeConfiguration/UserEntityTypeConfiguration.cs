using Domain.Entities;
using Domain.Primitives;
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

            builder.Property(x => x.Email).HasMaxLength(GlobalVariables.UserConstants.EmailMaxLength).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(GlobalVariables.UserConstants.NameMaxLength).IsRequired();
            builder.Property(x => x.Role).HasMaxLength(GlobalVariables.UserConstants.NameMaxLength).IsRequired(false);

        }
    }
}
