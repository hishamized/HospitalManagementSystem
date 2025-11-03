using HMS.Application.DTO.Bill;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.Bill
{
    public class GetBillsByPatientIdQuery : IRequest<List<BillListDto>>
    {
        public int PatientId { get; }

        public GetBillsByPatientIdQuery(int patientId)
        {
            PatientId = patientId;
        }
    }
}
