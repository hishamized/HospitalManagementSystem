using HMS.Application.DTO.PatientPortal;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.PatientPortal
{
    public class GetPatientVisitsPortalQuery : IRequest<IEnumerable<GetPatientVisitsResponseDto>>
    {
        public long PatientId;

        public GetPatientVisitsPortalQuery(long patientId)
        {
            PatientId = patientId;
        }
    }
}
