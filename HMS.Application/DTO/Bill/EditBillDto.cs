using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Bill
{
    public class EditBillDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int VisitId { get; set; }

        public DateTime BillDate { get; set; }

        public string PaymentStatus { get; set; }
        public string PaymentMode { get; set; }

        public decimal? RoomCharges { get; set; }
        public decimal? ProcedureCharges { get; set; }
        public decimal? MedicationCharges { get; set; }
        public decimal? ConsultationCharges { get; set; }
        public decimal? OpdConsultationFee { get; set; }

        public decimal? DiscountAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? NetAmount { get; set; }

        public string Notes { get; set; }

        // Optional audit fields
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}
