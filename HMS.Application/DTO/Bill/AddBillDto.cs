using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Bill
{
    public class AddBillDto
    {
        public int PatientId { get; set; }
        public int VisitId { get; set; }
        public string VisitType { get; set; } = string.Empty; // "Inpatient" or "Outpatient"

        public DateTime BillDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? NetAmount { get; set; }

        public string PaymentStatus { get; set; } = "Unpaid";
        public string? PaymentMode { get; set; }

        public decimal? RoomCharges { get; set; }
        public decimal? ProcedureCharges { get; set; }
        public decimal? MedicationCharges { get; set; }
        public decimal? ConsultationCharges { get; set; }
        public decimal? OpdConsultationFee { get; set; }

        public string? Notes { get; set; }
    }
}
