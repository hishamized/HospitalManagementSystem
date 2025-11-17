using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Payment
{
    public class AddPaymentDto
    {
        public int? PatientId { get; set; }
        public int? VisitId { get; set; }
        public int BillId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal? Amount { get; set; }

        public string PaymentStatus { get; set; }
        public string? PaymentMode { get; set; }
    }
}
