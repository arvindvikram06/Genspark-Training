using LibraryManagement.Models;

namespace LibraryManagement.Repository
{
    public interface IBookRepository
    {
        Book Add(Book book);
        IEnumerable<Book> GetAll();
        Book? GetById(int id);
        Book? Update(Book book);
        IEnumerable<Book> SearchByTitle(string title);
    }
}
