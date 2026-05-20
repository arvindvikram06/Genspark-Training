using LibraryManagement.Context;
using LibraryManagement.Models;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagement.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryDbContext _context;

        public BookRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public Book Add(Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();
            return book;
        }

        public IEnumerable<Book> GetAll()
        {
            return _context.Books.ToList();
        }

        public Book? GetById(int id)
        {
            return _context.Books.FirstOrDefault(b => b.BookId == id);
        }

        public Book? Update(Book book)
        {
            var existing = _context.Books.Find(book.BookId);
            if (existing == null) return null;

            existing.Title = book.Title;
            existing.Author = book.Author;
            existing.ISBN = book.ISBN;
            existing.PublishedYear = book.PublishedYear;
            existing.AvailableCopies = book.AvailableCopies;

            _context.SaveChanges();
            return existing;
        }

        public IEnumerable<Book> SearchByTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return GetAll();

            return _context.Books
                .Where(b => b.Title.ToLower().Contains(title.ToLower()))
                .ToList();
        }
    }
}
