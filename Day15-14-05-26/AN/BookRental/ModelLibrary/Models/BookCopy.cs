using System;
using System.Collections.Generic;

namespace ModelLibrary.Models
{
    public class BookCopy
    {
        public int CopyId { get; set; }
        public int BookId { get; set; }
        public string CopyNumber { get; set; } = string.Empty;
        public BookCopyStatus CopyStatus { get; set; }
        public DateTime AddedDate { get; set; }

        public Book Book { get; set; } = null!;
        public ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();
        public ICollection<BookDamageHistory> DamageHistories { get; set; } = new List<BookDamageHistory>();

        public override string ToString()
        {
            return $"CopyId: {CopyId}, BookId: {BookId}, CopyNumber: {CopyNumber}, Status: {CopyStatus}, AddedDate: {AddedDate}";
        }
    }
}
