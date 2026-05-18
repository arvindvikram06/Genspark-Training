using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModelLibrary.Models;

namespace DALLibrary.Configurations
{
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.HasKey(x => x.MemberId);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.HasIndex(x => x.PhoneNumber)
                .IsUnique();

            builder.HasOne(x => x.Membership)
                .WithMany(x => x.Members)
                .HasForeignKey(x => x.MembershipId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(m => m.MembershipStartDate)
                .HasColumnType("timestamp");

            builder.Property(m => m.MembershipEndDate)
                .HasColumnType("timestamp");
        }
    }
}