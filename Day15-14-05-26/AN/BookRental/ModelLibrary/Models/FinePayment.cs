using System;

namespace ModelLibrary.Models
{
    public class FinePayment
    {
        public int PaymentId { get; set; }
        public int FineId { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime PaidDate { get; set; }

        public Fine Fine { get; set; } = null!;
    }
}
