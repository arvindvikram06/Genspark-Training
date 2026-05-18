using System;

namespace ModelLibrary.Models
{
    public class MembershipPayment
    {
        public int PaymentId { get; set; }
        public int MemberId { get; set; }
        public int MembershipId { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime PaymentDate { get; set; }

        public Member Member { get; set; } = null!;
        public Membership Membership { get; set; } = null!;
    }
}
