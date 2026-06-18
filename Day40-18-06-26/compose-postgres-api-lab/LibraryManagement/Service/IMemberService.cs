using LibraryManagement.DTOs;
using LibraryManagement.Models;
using System.Collections.Generic;

namespace LibraryManagement.Service
{
    public interface IMemberService
    {
        Member AddMember(Member member, string password);
        IEnumerable<Member> GetAllMembers();
        Member GetMemberById(int id);
        LoginResponse Login(string email, string password);
    }
}
