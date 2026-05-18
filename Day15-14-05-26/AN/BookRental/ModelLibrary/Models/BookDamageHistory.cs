using System;

namespace ModelLibrary.Models
{
    public class BookDamageHistory
    {
        public int DamageId { get; set; }
        public int BookCopyId { get; set; }
        public int ReportedUserId { get; set; }
        public string DamageDescription { get; set; } = string.Empty;
        public DamageSeverity Severity { get; set; }
        public DateTime DamageDate { get; set; }

        public BookCopy BookCopy { get; set; } = null!;
        public Member ReportedUser { get; set; } = null!;

        public override string ToString()
        {
            return $"Damage ID: {DamageId} | User: {ReportedUserId} | Severity: {Severity} | Desc: {DamageDescription} | Date: {DamageDate}";
        }
    }
}
