

using DALLibrary.Contexts;
using DALLibrary.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.Models;

namespace DALLibrary.Repositories
{
    public class BookRepository
        : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(BookRentalContext context)
            : base(context)
        {
        }

        public override IEnumerable<Book> GetAll()
        {
            return _context.Books
                .Include(b => b.Category)
                .ToList();
        }

        public override Book? GetById(int id)
        {
            return _context.Books
                .Include(b => b.Category)
                .FirstOrDefault(b => b.BookId == id);
        }

        public IEnumerable<Book> SearchBooksByTitle(string title)
        {
            return _context.Books
                .Include(b => b.Category)
                .Where(b =>
                    EF.Functions.ILike(b.Title, $"%{title}%"))
                .ToList();
        }

        public IEnumerable<Book> SearchBooksByAuthor(string author)
        {
            return _context.Books
                .Include(b => b.Category)
                .Where(b =>
                    EF.Functions.ILike(b.Author, $"%{author}%"))
                .ToList();
        }

        public IEnumerable<Book> SearchBooksByCategory(string category)
        {
            return _context.Books
                .Include(b => b.Category)
                .Where(b => EF.Functions.ILike(b.Category.CategoryName, $"%{category}%"))
                .ToList();
        }

        public bool BookExists(int bookId)
        {
            return _context.Books
                .Any(b => b.BookId == bookId);
        }
    }
}