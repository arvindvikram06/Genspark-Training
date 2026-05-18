using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModelLibrary.Models;

namespace DALLibrary.Configurations
{
    public class BookCopyConfiguration : IEntityTypeConfiguration<BookCopy>
    {
        public void Configure(EntityTypeBuilder<BookCopy> builder)
        {
            builder.HasKey(x => x.CopyId);

            builder.Property(bc => bc.AddedDate)
                .HasColumnType("timestamp");

            builder.HasOne(x => x.Book)
                .WithMany(x => x.BookCopies)
                .HasForeignKey(x => x.BookId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
