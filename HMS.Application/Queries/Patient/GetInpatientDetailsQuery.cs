using HMS.Application.DTO.Patient;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.Patient
{
    public class GetInpatientDetailsQuery : IRequest<InpatientDetailsDto?>
    {
        public int PatientId { get; set; }
        public int VisitId { get; set; }

        public GetInpatientDetailsQuery(int patientId, int visitId)
        {
            PatientId = patientId;
            VisitId = visitId;
        }
    }
}
