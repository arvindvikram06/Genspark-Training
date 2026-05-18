using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModelLibrary.Models;

namespace DALLibrary.Configurations
{
    public class BookDamageHistoryConfiguration : IEntityTypeConfiguration<BookDamageHistory>
    {
        public void Configure(EntityTypeBuilder<BookDamageHistory> builder)
        {
            builder.HasKey(x => x.DamageId);

            builder.Property(dh => dh.DamageDate)
                .HasColumnType("timestamp");

            builder.HasOne(x => x.BookCopy)
                .WithMany(x => x.DamageHistories)
                .HasForeignKey(x => x.BookCopyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ReportedUser)
                .WithMany(x => x.DamageHistories)
                .HasForeignKey(x => x.ReportedUserId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
