using DALLibrary.Contexts;
using DALLibrary.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.Models;

namespace DALLibrary.Repositories
{
    public class MembershipRepository : GenericRepository<Membership>, IMembershipRepository
    {
    
        public MembershipRepository(BookRentalContext context) : base(context)
        {
        }


        public override IEnumerable<Membership> GetAll()
        {
            return _context.Memberships.IgnoreQueryFilters();
        }

    }
}
