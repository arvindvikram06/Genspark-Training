using ModelLibrary.Models;
using System.Collections.Generic;

namespace BLLibrary.Interfaces
{
    public interface IBookService
    {
        void AddBook(Book book);
        void AddBookCopies(int bookId, int numberOfCopies);
        Book? GetBookById(int bookId);
        IEnumerable<Book> GetAllBooks();
        IEnumerable<Book> SearchBooks(string query);
        IEnumerable<Book> SearchBooksByCategory(string category);
        IEnumerable<Book> GetAvailableBooks();
        IEnumerable<BookCopy> GetBookCopies(int bookId);
        void MarkCopyAsDamaged(int copyId, int userId, string description, DamageSeverity severity);

        void MarkCopyAsAvailable(int copyId);
        IEnumerable<BookDamageHistory> GetDamageHistory(int copyId);
    }
}
