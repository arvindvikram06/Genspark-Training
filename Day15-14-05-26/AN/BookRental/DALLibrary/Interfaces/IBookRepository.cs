
using ModelLibrary.Models;

namespace DALLibrary.Interfaces
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        public IEnumerable<Book> SearchBooksByAuthor(string author);

        public IEnumerable<Book> SearchBooksByTitle(string title);
        public IEnumerable<Book> SearchBooksByCategory(string category);

        bool BookExists(int bookid);

    }
}