using System;

namespace LibraryManagement.Models
{
    public class Member
    {
        public int MemberId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime MembershipDate { get; set; } = DateTime.UtcNow;

        public Member()
        {
        }

        public Member(string fullName, string email, string phoneNumber, DateTime membershipDate)
        {
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            MembershipDate = membershipDate;
        }
    }
}
