using System;
using System.Collections.Generic;

namespace ModelLibrary.Models
{
    public class Member
    {
        public int MemberId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int? MembershipId { get; set; }
        public MembershipStatus MembershipStatus { get; set; } = MembershipStatus.NOT_PURCHASED;
        public DateTime? MembershipStartDate { get; set; }
        public DateTime? MembershipEndDate { get; set; }

        public bool IsActive {get; set;} = true;

        public Membership? Membership { get; set; }
        public ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();
        public ICollection<MembershipPayment> MembershipPayments { get; set; } = new List<MembershipPayment>();
        public ICollection<BookDamageHistory> DamageHistories { get; set; } = new List<BookDamageHistory>();


        public override string ToString()
        {
            return $"\n------\n" +
                   $"ID: {MemberId}\n" +
                   $"Name: {Name}\n" +
                   $"Email: {Email}\n" +
                   $"Phone: {PhoneNumber}\n" +
                   $"Status: {(IsActive ? "Active" : "Inactive")}";
        }
    }
}