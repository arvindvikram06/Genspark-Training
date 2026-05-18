using ModelLibrary.Models;
using System.Collections.Generic;

namespace BLLibrary.Interfaces
{
    public interface IBorrowingService
    {
        void BorrowBook(int memberId, int bookId);
        void ReturnBook(int memberId, int copyId);
        IEnumerable<Borrowing> GetBorrowedBooksByMember(int memberId);
    }
}
