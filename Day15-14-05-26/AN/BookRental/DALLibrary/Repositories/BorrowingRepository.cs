using DALLibrary.Contexts;
using DALLibrary.Interfaces;
using ModelLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DALLibrary.Repositories
{
    public class BorrowingRepository : GenericRepository<Borrowing>, IBorrowingRepository
    {
        public BorrowingRepository(BookRentalContext context) : base(context)
        {
        }

        public IEnumerable<Borrowing> GetBorrowingsByMemberId(int memberId)
        {
            return _dbSet
                .Include(b => b.BookCopy)
                    .ThenInclude(bc => bc.Book)
                .Where(b => b.MemberId == memberId)
                .ToList();
        }

        public IEnumerable<Borrowing> GetOverdueBorrowings()
        {
            return _dbSet.Where(b => b.Status == BorrowingStatus.ACTIVE && b.DueDate < System.DateTime.Now).ToList();
        }

        public IEnumerable<Borrowing> GetCurrentlyBorrowedBooks()
        {
            return _dbSet.Where(b => b.Status == BorrowingStatus.ACTIVE).ToList();
        }
    }
}

