using ModelLibrary.Models;

namespace DALLibrary.Interfaces
{
    public interface IBookCopyRepository
    {
        void AddBookCopy(BookCopy copy);

        BookCopy? GetBookCopyById(int copyId);

        IEnumerable<BookCopy> GetCopiesByBookId(int bookId);

        IEnumerable<BookCopy> GetAvailableCopies(int bookId);

        BookCopy? GetAvailableCopy(int bookId);

        void UpdateBookCopy(BookCopy copy);
    }
}