using LibraryManagement.DTOs;
using LibraryManagement.Exceptions;
using LibraryManagement.Models;
using LibraryManagement.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LibraryManagement.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpPost]
        public IActionResult AddBook([FromBody] CreateBookDto dto)
        {
            try
            {
                var book = new Book
                {
                    Title = dto.Title,
                    Author = dto.Author,
                    PublishedYear = dto.PublishedYear,
                    AvailableCopies = dto.AvailableCopies
                };
                _bookService.AddBook(book);
                return Ok(new { message = "Book added successfully" });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetAllBooks()
        {
            try
            {
                var books = _bookService.GetAllBooks();
                return Ok(books);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id:int}")]
        public ActionResult<Book> GetBookById(int id)
        {
            try
            {
                var book = _bookService.GetBookById(id);
                return Ok(book);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<Book>> SearchBooks([FromQuery] string title)
        {
            try
            {
                var books = _bookService.SearchBooksByTitle(title);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private ActionResult HandleException(Exception ex)
        {
            if (ex is EntityNotFoundException)
            {
                return NotFound(new { message = ex.Message });
            }
            if (ex is InvalidInputException)
            {
                return BadRequest(new { message = ex.Message });
            }
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
