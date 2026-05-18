using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModelLibrary.Models;

namespace DALLibrary.Configurations
{
    public class FineConfiguration : IEntityTypeConfiguration<Fine>
    {
        public void Configure(EntityTypeBuilder<Fine> builder)
        {
            builder.HasKey(x => x.FineId);

            builder.Property(x => x.FineAmount)
                .HasColumnType("decimal(10,2)");

            builder.HasOne(x => x.Borrowing)
                .WithMany(x => x.Fines)
                .HasForeignKey(x => x.BorrowingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(f => f.CreatedAt)
                .HasColumnType("timestamp");
        }
    }
}
