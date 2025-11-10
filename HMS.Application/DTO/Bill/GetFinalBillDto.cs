using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Bill
{
    public class GetFinalBillDto
    {
        public decimal TotalAmountFinal { get; set; }
        public decimal DiscountAmountFinal { get; set; }

        public decimal NetAmountFinal { get; set; }
        public decimal RoomChargesFinal { get; set; }
        public decimal ProcedureChargesFinal { get; set; }
        public decimal MedicationChargesFinal { get; set; }

        public decimal OpdConsultationFeeFinal { get; set; }
        public decimal ConsultationChargesFinal { get; set; }
    }
}
