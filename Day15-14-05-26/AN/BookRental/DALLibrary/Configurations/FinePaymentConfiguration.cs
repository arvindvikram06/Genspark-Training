using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModelLibrary.Models;

namespace DALLibrary.Configurations
{
    public class FinePaymentConfiguration : IEntityTypeConfiguration<FinePayment>
    {
        public void Configure(EntityTypeBuilder<FinePayment> builder)
        {
            builder.HasKey(x => x.PaymentId);

            builder.Property(x => x.PaidAmount)
                .HasColumnType("decimal(10,2)");

            builder.Property(fp => fp.PaidDate)
                .HasColumnType("timestamp");

            builder.HasOne(x => x.Fine)
                .WithMany(x => x.FinePayments)
                .HasForeignKey(x => x.FineId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
