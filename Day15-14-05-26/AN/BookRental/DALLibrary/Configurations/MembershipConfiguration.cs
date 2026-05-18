using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModelLibrary.Models;

namespace DALLibrary.Configurations
{
    public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
    {
        public void Configure(EntityTypeBuilder<Membership> builder)
        {
            builder.HasKey(x => x.MembershipId);

            builder.Property(x => x.MembershipPrice)
                .HasColumnType("decimal(10,2)");
            builder.HasQueryFilter(m => m.IsActive);

        }
    }
}
