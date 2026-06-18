using LibraryManagement.Context;
using LibraryManagement.Models;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagement.Repository
{
    public class MemberRepository : IMemberRepository
    {
        private readonly LibraryDbContext _context;

        public MemberRepository(LibraryDbContext context)
        {
            _context = context;
        }
        public Member Add(Member member)
        {
            _context.Members.Add(member);
            _context.SaveChanges();
            return member;
        }
        public IEnumerable<Member> GetAll()
        {
            return _context.Members.ToList();
        }

        public Member? GetById(int id)
        {
            return _context.Members.FirstOrDefault(m => m.MemberId == id);
        }

        public Member? GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            return _context.Members.FirstOrDefault(m => m.Email.ToLower() == email.ToLower());
        }

    }
}
