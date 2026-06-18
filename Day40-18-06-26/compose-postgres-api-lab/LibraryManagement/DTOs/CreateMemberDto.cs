using System;

namespace LibraryManagement.DTOs
{
    public class CreateMemberDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime MembershipDate { get; set; }
    }
}
