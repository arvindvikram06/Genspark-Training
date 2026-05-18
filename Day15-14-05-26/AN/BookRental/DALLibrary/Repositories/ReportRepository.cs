using DALLibrary.Contexts;
using DALLibrary.Interfaces;
using ModelLibrary.Models;
using ModelLibrary.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DALLibrary.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly BookRentalContext _context;

        public ReportRepository(BookRentalContext context)
        {
            _context = context;
        }

        public IEnumerable<Borrowing> GetCurrentlyBorrowedBooks()
        {
            return _context.Borrowings
                .Include(b => b.Member)
                .Include(b => b.BookCopy)
                    .ThenInclude(bc => bc.Book)
                .Where(b => b.Status == BorrowingStatus.ACTIVE)
                .ToList();
        }

        public IEnumerable<Borrowing> GetOverdueBooks()
        {
            return _context.Borrowings
                .Include(b => b.Member)
                .Include(b => b.BookCopy)
                    .ThenInclude(bc => bc.Book)
                .Where(b => b.Status == BorrowingStatus.ACTIVE && b.DueDate < DateTime.Now)
                .ToList();
        }

        public IEnumerable<MemberPendingFineDto> GetMembersWithPendingFines()
        {
            return _context.Members
                .Where(m => m.Borrowings.Any(b => b.Fines.Any(f => f.PaidStatus == FineStatus.PENDING)))
                .Select(m => new MemberPendingFineDto
                {
                    MemberId = m.MemberId,
                    MemberName = m.Name,
                    MemberEmail = m.Email,
                    TotalPendingFine = m.Borrowings
                        .SelectMany(b => b.Fines)
                        .Where(f => f.PaidStatus == FineStatus.PENDING)
                        .Sum(f => f.FineAmount)
                })
                .ToList();
        }

        public IEnumerable<MostBorrowedBookDto> GetMostBorrowedBooks(int limit = 10)
        {
            return _context.Books
                .Select(b => new MostBorrowedBookDto
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Author = b.Author,
                    BorrowCount = b.BookCopies.SelectMany(bc => bc.Borrowings).Count()
                })
                .OrderByDescending(x => x.BorrowCount)
                .Take(limit)
                .ToList();
        }

        public IEnumerable<AvailableBooksByCategoryDto> GetAvailableBooksByCategory()
        {
            return _context.BookCategories
                .Select(c => new AvailableBooksByCategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    AvailableCopiesCount = c.Books
                        .SelectMany(b => b.BookCopies)
                        .Count(bc => bc.CopyStatus == BookCopyStatus.AVAILABLE)
                })
                .ToList();
        }

        public IEnumerable<Borrowing> GetMemberBorrowingHistory(int memberId)
        {
            return _context.Borrowings
                .Include(b => b.BookCopy)
                    .ThenInclude(bc => bc.Book)
                .Where(b => b.MemberId == memberId)
                .OrderByDescending(b => b.BorrowDate)
                .ToList();
        }
    }
}
