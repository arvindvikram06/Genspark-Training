using LibraryManagement.Models;
using System.Collections.Generic;

namespace LibraryManagement.Service
{
    public interface IBookService
    {
        Book AddBook(Book book);
        IEnumerable<Book> GetAllBooks();
        Book GetBookById(int id);
        IEnumerable<Book> SearchBooksByTitle(string title);
    }
}
