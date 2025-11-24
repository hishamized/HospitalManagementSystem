using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities
{
    public class Bill
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public int? VisitId { get; set; } // FK to PatientVisit
        public string VisitType { get; set; } = string.Empty; // "Inpatient" or "Outpatient"

        public DateTime BillDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? NetAmount { get; set; }

        // Payment Info
        public string PaymentStatus { get; set; } = "Unpaid"; // e.g., Paid / Pending / Partial
        public string? PaymentMode { get; set; } // Cash / Card / Insurance

        // Optional Inpatient-only fields
        public decimal? RoomCharges { get; set; }
        public decimal? ProcedureCharges { get; set; }
        public decimal? MedicationCharges { get; set; }
        public decimal? ConsultationCharges { get; set; }

        // Optional Outpatient-only fields
        public decimal? OpdConsultationFee { get; set; }

        public string? Notes { get; set; }

        // System fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public PatientVisit? PatientVisit { get; set; }

    }

}
