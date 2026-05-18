using DALLibrary.Contexts;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.Models;

namespace DALLibrary.Repositories
{
    public class MemberRepository
        : GenericRepository<Member>, IMemberRepository
    {
        public MemberRepository(BookRentalContext context)
            : base(context)
        {
        }

        public override IEnumerable<Member> GetAll()
        {
            return _context.Members
                .Include(m => m.Membership)
                .ToList();
        }

        public override Member? GetById(int id)
        {
            return _context.Members
                .Include(m => m.Membership)
                .FirstOrDefault(m => m.MemberId == id);
        }

        public Member? GetMemberByEmail(string email)
        {
            return _context.Members
                .Include(m => m.Membership)
                .FirstOrDefault(m => m.Email == email);
        }

        public Member? GetMemberByPhoneNumber(string phoneNumber)
        {
            return _context.Members
                .Include(m => m.Membership)
                .FirstOrDefault(m => m.PhoneNumber == phoneNumber);
        }

        public bool MemberExists(int memberId)
        {
            return _context.Members
                .Any(m => m.MemberId == memberId);
        }
    }
}