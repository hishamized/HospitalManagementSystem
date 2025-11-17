using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int? PatientId { get; set; }
        public int? VisitId { get; set; }
        public int BillId { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public decimal? Amount { get; set; }

        public string PaymentStatus { get; set; } = "Unpaid"; // e.g., Paid / Pending / Partial
        public string? PaymentMode { get; set; } // Cash / Card / Insurance
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual Bill Bill { get; set; } = null!;

    }
}
