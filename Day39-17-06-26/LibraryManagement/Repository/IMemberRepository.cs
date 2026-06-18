using LibraryManagement.Models;
using System.Collections.Generic;

namespace LibraryManagement.Repository
{
    public interface IMemberRepository
    {
        Member Add(Member member);
        IEnumerable<Member> GetAll();
        Member? GetById(int id);
        Member? GetByEmail(string email);
    }
}
