using ModelLibrary.Models;
using ModelLibrary.DTOs;
using System.Collections.Generic;

namespace DALLibrary.Interfaces
{
    public interface IReportRepository
    {
        IEnumerable<Borrowing> GetCurrentlyBorrowedBooks();
        IEnumerable<Borrowing> GetOverdueBooks();
        IEnumerable<MemberPendingFineDto> GetMembersWithPendingFines();
        IEnumerable<MostBorrowedBookDto> GetMostBorrowedBooks(int limit = 10);
        IEnumerable<AvailableBooksByCategoryDto> GetAvailableBooksByCategory();
        IEnumerable<Borrowing> GetMemberBorrowingHistory(int memberId);
    }
}
