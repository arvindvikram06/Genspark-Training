using LibraryManagement.Models;

namespace LibraryManagement.Repository
{
    public interface IMemberRepository
    {
        Member Add(Member member);
        IEnumerable<Member> GetAll();
        Member? GetById(int id);
    }
}
