using System;
using System.Collections.Generic;

namespace ModelLibrary.Models
{
    public class Borrowing
    {
        public int BorrowingId { get; set; }
        public int MemberId { get; set; }
        public int CopyId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public BorrowingStatus Status { get; set; }

        public Member Member { get; set; } = null!;
        public BookCopy BookCopy { get; set; } = null!;
        public ICollection<Fine> Fines { get; set; } = new List<Fine>();

        public override string ToString()
        {
            return $"\nBorrowing ID: {BorrowingId} | Copy ID: {CopyId} |\nBook: '{BookCopy?.Book?.Title}' by {BookCopy?.Book?.Author}|\nBorrowed: {BorrowDate.ToShortDateString()} | Due: {DueDate.ToShortDateString()} |\nStatus: {Status}";
        }
    }
}
