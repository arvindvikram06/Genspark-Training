using ModelLibrary.Models;

namespace DALLibrary.Interfaces
{
    public interface IBorrowingRepository : IGenericRepository<Borrowing>
    {
        IEnumerable<Borrowing> GetBorrowingsByMemberId(int memberId);
        IEnumerable<Borrowing> GetOverdueBorrowings();
        IEnumerable<Borrowing> GetCurrentlyBorrowedBooks();
    }
}
