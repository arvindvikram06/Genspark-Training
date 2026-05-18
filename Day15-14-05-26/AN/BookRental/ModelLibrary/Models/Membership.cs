using System;
using System.Collections.Generic;

namespace ModelLibrary.Models
{
    public class Membership
    {
        public int MembershipId { get; set; }
        public string MembershipName { get; set; } = string.Empty;
        public decimal MembershipPrice { get; set; }
        public int BorrowLimit { get; set; }
        public int BorrowDaysLimit { get; set; }

        public bool IsActive { get; set; } = true; 
        public ICollection<Member> Members { get; set; } = new List<Member>();

        public override string ToString()
        {
            return $"ID: {MembershipId}, Name: {MembershipName}, Price: {MembershipPrice}, Borrow Limit: {BorrowLimit}, Borrow Days Limit: {BorrowDaysLimit}, Status:{(IsActive ? "Active" : "Inactive")}";
        }
    }
}
