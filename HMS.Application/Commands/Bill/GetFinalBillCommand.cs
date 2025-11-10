using HMS.Application.DTO.Bill;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Bill
{
    public class GetFinalBillCommand : IRequest<GetFinalBillDto>
    {
        public int PatientId { get; set; }
        public int VisitId { get; set; }

        public GetFinalBillCommand(int patientId, int visitId) {
            PatientId = patientId;
            VisitId = visitId;
        }
    }
}
