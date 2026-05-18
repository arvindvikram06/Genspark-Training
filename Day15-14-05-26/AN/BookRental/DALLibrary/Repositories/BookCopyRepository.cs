using DALLibrary.Contexts;
using DALLibrary.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.Models;

namespace DALLibrary.Repositories
{
    public class BookCopyRepository : IBookCopyRepository
    {
        private readonly BookRentalContext _context;

        public BookCopyRepository(BookRentalContext context)
        {
            _context = context;
        }

        public void AddBookCopy(BookCopy copy)
        {
            _context.BookCopies.Add(copy);
        }

        public BookCopy? GetBookCopyById(int copyId)
        {
            return _context.BookCopies
                .Include(c => c.Book)
                .FirstOrDefault(c => c.CopyId == copyId);
        }

        public IEnumerable<BookCopy> GetCopiesByBookId(int bookId)
        {
            return _context.BookCopies
                .Where(c => c.BookId == bookId)
                .ToList();
        }

        public IEnumerable<BookCopy> GetAvailableCopies(int bookId)
        {
            return _context.BookCopies
                .Where(c =>
                    c.BookId == bookId &&
                    c.CopyStatus == BookCopyStatus.AVAILABLE)
                .ToList();
        }

        public BookCopy? GetAvailableCopy(int bookId)
        {
            return _context.BookCopies
                .FirstOrDefault(c =>
                    c.BookId == bookId &&
                    c.CopyStatus == BookCopyStatus.AVAILABLE);
        }


        public void UpdateBookCopy(BookCopy copy)
        {
            _context.BookCopies.Update(copy);
        }
    }
}