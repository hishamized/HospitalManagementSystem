using HMS.Application.DTO.MedicalHistory;
using MediatR;
using System.Collections.Generic;

namespace HMS.Application.Queries.MedicalHistory
{
    public class GetPatientMedicalHistoryQuery : IRequest<IEnumerable<GetPatientMedicalHistoryDto>>
    {
        public int PatientId { get; }

        public GetPatientMedicalHistoryQuery(int patientId)
        {
            PatientId = patientId;
        }
    }
}
