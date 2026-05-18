using DALLibrary.Contexts;
using DALLibrary.Interfaces;
using ModelLibrary.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DALLibrary.Repositories
{
    public class FineRepository : GenericRepository<Fine>, IFineRepository
    {
        public FineRepository(BookRentalContext context) : base(context)
        {
        }

        public IEnumerable<Fine> GetFinesByMemberId(int memberId)
        {
            return _dbSet
                .Include(f => f.Borrowing)
                .Where(f => f.Borrowing.MemberId == memberId)
                .ToList();
        }
    }
}
