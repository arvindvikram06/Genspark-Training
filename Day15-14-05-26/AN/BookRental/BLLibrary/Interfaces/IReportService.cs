using ModelLibrary.Models;
using ModelLibrary.DTOs;
using System.Collections.Generic;

namespace BLLibrary.Interfaces
{
    public interface IReportService
    {
        IEnumerable<Borrowing> GetCurrentlyBorrowedBooks();
        IEnumerable<Borrowing> GetOverdueBooks();
        IEnumerable<MostBorrowedBookDto> GetMostBorrowedBooks();
        IEnumerable<AvailableBooksByCategoryDto> GetAvailableBooksByCategory();
        IEnumerable<Borrowing> GetMemberBorrowingHistory(int memberId);
        IEnumerable<MemberPendingFineDto> GetMembersWithPendingFines();
    }
}
