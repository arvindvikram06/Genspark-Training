using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModelLibrary.Models;

namespace DALLibrary.Configurations
{
    public class BorrowingConfiguration : IEntityTypeConfiguration<Borrowing>
    {
        public void Configure(EntityTypeBuilder<Borrowing> builder)
        {
            builder.HasKey(x => x.BorrowingId);

            builder.Property(b => b.BorrowDate)
                .HasColumnType("timestamp");

            builder.Property(b => b.DueDate)
                .HasColumnType("timestamp");

            builder.Property(b => b.ReturnedDate)
                .HasColumnType("timestamp");

            builder.HasOne(x => x.Member)
                .WithMany(x => x.Borrowings)
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BookCopy)
                .WithMany(x => x.Borrowings)
                .HasForeignKey(x => x.CopyId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
