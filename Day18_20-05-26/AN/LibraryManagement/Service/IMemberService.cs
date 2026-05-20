using LibraryManagement.Models;
using System.Collections.Generic;

namespace LibraryManagement.Service
{
    public interface IMemberService
    {
        Member AddMember(Member member);
        IEnumerable<Member> GetAllMembers();
        Member GetMemberById(int id);
    }
}
