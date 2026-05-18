using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModelLibrary.Models;

namespace DALLibrary.Configurations
{
    public class MembershipPaymentConfiguration : IEntityTypeConfiguration<MembershipPayment>
    {
        public void Configure(EntityTypeBuilder<MembershipPayment> builder)
        {
            builder.HasKey(x => x.PaymentId);

            builder.Property(x => x.PaidAmount)
                .HasColumnType("decimal(10,2)");

            builder.Property(mp => mp.PaymentDate)
                .HasColumnType("timestamp");

            builder.Property(mp => mp.StartDate)
                .HasColumnType("timestamp");

            builder.Property(mp => mp.EndDate)
                .HasColumnType("timestamp");

            builder.HasOne(x => x.Member)
                .WithMany(x => x.MembershipPayments)
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Membership)
                .WithMany()
                .HasForeignKey(x => x.MembershipId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
