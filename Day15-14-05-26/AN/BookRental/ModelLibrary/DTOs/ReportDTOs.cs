using System;

namespace ModelLibrary.DTOs
{
    public class MostBorrowedBookDto
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int BorrowCount { get; set; }
    }

    public class AvailableBooksByCategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int AvailableCopiesCount { get; set; }
    }

    public class MemberPendingFineDto
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string MemberEmail { get; set; } = string.Empty;
        public decimal TotalPendingFine { get; set; }
    }
}
