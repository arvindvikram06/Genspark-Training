using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ModelLibrary.Models
{
    public class Fine
    {
        public int FineId { get; set; }
        public int BorrowingId { get; set; }
        public decimal FineAmount { get; set; }
        public FineReason Reason { get; set; }
        public FineStatus PaidStatus { get; set; }

        public DateTime CreatedAt { get; set; }

        public Borrowing Borrowing { get; set; } = null!;
        public ICollection<FinePayment> FinePayments { get; set; } = new List<FinePayment>();

        public override string ToString()
        {
            return $"Fine ID: {FineId} | Amount: ₹{FineAmount} | Reason: {Reason} | Status: {PaidStatus}";
        }
    }
}
