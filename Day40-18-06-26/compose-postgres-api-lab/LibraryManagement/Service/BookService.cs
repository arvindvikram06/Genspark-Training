using LibraryManagement.Exceptions;
using LibraryManagement.Models;
using LibraryManagement.Repository;
using System.Collections.Generic;

namespace LibraryManagement.Service
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public Book AddBook(Book book)
        {
            book.BookId = 0;
            if (string.IsNullOrWhiteSpace(book.Title))
            {
                throw new InvalidInputException("Book title should not be empty.");
            }

            if (string.IsNullOrWhiteSpace(book.Author))
            {
                throw new InvalidInputException("Author name should not be empty.");
            }

            if (book.AvailableCopies < 0)
            {
                throw new InvalidInputException("Available copies should be greater than or equal to 0.");
            }

            book.ISBN = Guid.NewGuid().ToString();

            var savedBook = _bookRepository.Add(book);

            savedBook.ISBN = $"{DateTime.Now.Year}-{savedBook.BookId:D5}";
            _bookRepository.Update(savedBook);

            return savedBook;
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return _bookRepository.GetAll();
        }

        public Book GetBookById(int id)
        {
            var book = _bookRepository.GetById(id);
            if (book == null)
            {
                throw new EntityNotFoundException("Book", id);
            }
            return book;
        }

        public IEnumerable<Book> SearchBooksByTitle(string title)
        {
            return _bookRepository.SearchByTitle(title);
        }
    }
}
