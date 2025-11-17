using HMS.Application.DTO.PatientPortal;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.Bill
{
    public class GetPatientBillsQuery : IRequest<IEnumerable<GetPatientBillsDto>>
    {
        public long PatientId;
        public long VisitId;
        public GetPatientBillsQuery(long patientId, long visitId) {
            PatientId = patientId;
            VisitId = visitId;
        }
    }
}
