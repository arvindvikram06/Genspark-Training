using ModelLibrary.Models;
using BLLibrary.Interfaces;
using DALLibrary.Interfaces;
using BLLibrary.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System;
using DALLibrary.Contexts;
using DALLibrary.Exceptions;

namespace BLLibrary.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookCopyRepository _bookCopyRepository;
        private readonly IBookDamageHistoryRepository _damageRepository;
        private readonly IFineRepository _fineRepository;
        private readonly IBorrowingRepository _borrowingRepository;
        private readonly BookRentalContext _context;

        public BookService(
            IBookRepository bookRepository, 
            IBookCopyRepository bookCopyRepository,
            IBookDamageHistoryRepository damageRepository,
            IFineRepository fineRepository,
            IBorrowingRepository borrowingRepository,
            BookRentalContext context)
        {
            _bookRepository = bookRepository;
            _bookCopyRepository = bookCopyRepository;
            _damageRepository = damageRepository;
            _fineRepository = fineRepository;
            _borrowingRepository = borrowingRepository;
            _context = context;
        }

        public void AddBook(Book book)
        {
            if (string.IsNullOrWhiteSpace(book.Title) || string.IsNullOrWhiteSpace(book.Author))
            {
                throw new ValidationException("Book title and author cannot be empty.");
            }
            try
            {
                _bookRepository.Add(book);
                _context.SaveChanges();
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Unable to add book.", ex);
            }
        }

        public void AddBookCopies(int bookId, int numberOfCopies)
        {
            var book = _bookRepository.GetById(bookId);
            if (book == null) throw new DataNotFoundException("Book", bookId);

            try
            {
                for (int i = 0; i < numberOfCopies; i++)
                {
                    var copy = new BookCopy
                    {
                        BookId = bookId,
                        CopyNumber = Guid.NewGuid().ToString().Substring(0, 8),
                        CopyStatus = BookCopyStatus.AVAILABLE,
                        AddedDate = DateTime.Now
                    };
                    _bookCopyRepository.AddBookCopy(copy);
                }
                _context.SaveChanges();
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Failed to add book copies.", ex);
            }
        }

        public Book? GetBookById(int bookId)
        {
            return _bookRepository.GetById(bookId);
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return _bookRepository.GetAll();
        }

        public IEnumerable<Book> SearchBooks(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return GetAllBooks();
            query = query.ToLower();
            
            var allBooks = _bookRepository.GetAll();
            return allBooks.Where(b => 
                (b.Title != null && b.Title.ToLower().Contains(query)) ||
                (b.Author != null && b.Author.ToLower().Contains(query)));
        }

        public IEnumerable<Book> SearchBooksByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category)) return new List<Book>();
            return _bookRepository.SearchBooksByCategory(category);
        }

        public IEnumerable<Book> GetAvailableBooks()
        {
            var allBooks = _bookRepository.GetAll();
            return allBooks.Where(b => _bookCopyRepository.GetAvailableCopies(b.BookId).Any());
        }

        public void MarkCopyAsDamaged(int copyId, int userId, string description, DamageSeverity severity)
        {
            var copy = _bookCopyRepository.GetBookCopyById(copyId);
            if (copy == null) throw new DataNotFoundException("BookCopy", copyId);

            try
            {
                copy.CopyStatus = BookCopyStatus.DAMAGED;
                _bookCopyRepository.UpdateBookCopy(copy);

                var damageHistory = new BookDamageHistory
                {
                    BookCopyId = copyId,
                    ReportedUserId = userId,
                    DamageDescription = description,
                    Severity = severity,
                    DamageDate = DateTime.Now
                };
                _damageRepository.Add(damageHistory);

                var lastBorrowing = _borrowingRepository.GetAll()
                    .Where(b => b.CopyId == copyId && b.MemberId == userId)
                    .OrderByDescending(b => b.BorrowDate)
                    .FirstOrDefault();

                if (lastBorrowing != null)
                {
                    decimal fineAmount = severity switch
                    {
                        DamageSeverity.LOW => 100m,
                        DamageSeverity.MEDIUM => 300m,
                        DamageSeverity.HIGH => 1000m,
                        _ => 200m
                    };

                    var fine = new Fine
                    {
                        BorrowingId = lastBorrowing.BorrowingId,
                        FineAmount = fineAmount,
                        Reason = FineReason.DAMAGED_BOOK,
                        PaidStatus = FineStatus.PENDING,
                        CreatedAt = DateTime.Now
                    };
                    _fineRepository.Add(fine);
                }

                _context.SaveChanges();
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Failed to mark copy as damaged.", ex);
            }
        }

        public void MarkCopyAsAvailable(int copyId)
        {
            var copy = _bookCopyRepository.GetBookCopyById(copyId);
            if (copy == null) throw new DataNotFoundException("BookCopy", copyId);

            try
            {
                copy.CopyStatus = BookCopyStatus.AVAILABLE;
                _context.SaveChanges();
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Failed to mark copy as available.", ex);
            }

        }


        public IEnumerable<BookCopy> GetBookCopies(int bookId)
        {
            return _bookCopyRepository.GetCopiesByBookId(bookId);
        }

        public IEnumerable<BookDamageHistory> GetDamageHistory(int copyId)
        {
            return _damageRepository.GetDamageHistoryByBookCopyId(copyId);
        }
    }
}
