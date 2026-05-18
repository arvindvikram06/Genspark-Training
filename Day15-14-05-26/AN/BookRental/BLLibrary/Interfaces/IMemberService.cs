

using ModelLibrary.Models;

namespace BLLibrary.Interfaces{

    public interface IMemberService{

        public void RegisterMember(string name,string email, string phoneNumber);

        public void UpdateMember(int memberId,string field, string newValue);

        public void DeactivateMember(int memberId);

        public IEnumerable<Member> GetAllMembers();

        public Member GetMemberById(int memberId);

        public Member GetMemberByEmail(string email);

        public Member GetMemberByPhoneNumber(string phoneNumber);
    }
}
