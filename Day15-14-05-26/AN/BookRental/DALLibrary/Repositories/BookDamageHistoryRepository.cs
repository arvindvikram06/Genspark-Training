using DALLibrary.Contexts;
using DALLibrary.Interfaces;
using ModelLibrary.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DALLibrary.Repositories
{
    public class BookDamageHistoryRepository : GenericRepository<BookDamageHistory>, IBookDamageHistoryRepository
    {
        public BookDamageHistoryRepository(BookRentalContext context) : base(context)
        {
        }

        public IEnumerable<BookDamageHistory> GetDamageHistoryByBookCopyId(int copyId)
        {
            return _dbSet
                .Include(d => d.ReportedUser)
                .Where(d => d.BookCopyId == copyId)
                .ToList();
        }
    }
}
