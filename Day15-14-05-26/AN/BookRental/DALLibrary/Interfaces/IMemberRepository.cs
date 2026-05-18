
using DALLibrary.Interfaces;
using ModelLibrary.Models;

namespace DALLibrary
{
    public interface IMemberRepository : IGenericRepository<Member>
    {
        Member? GetMemberByEmail(string email);

        Member? GetMemberByPhoneNumber(string phoneNumber);

        bool MemberExists(int memberId);
    }
}