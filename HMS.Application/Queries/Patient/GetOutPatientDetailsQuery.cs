using MediatR;
using HMS.Application.DTO.Patient;

namespace HMS.Application.Queries.Patient
{
    public class GetOutPatientDetailsQuery : IRequest<OutPatientDetailsDto?>
    {
        public int PatientId { get; set; }
        public int VisitId { get; set; }

        public GetOutPatientDetailsQuery(int patientId, int visitId)
        {
            PatientId = patientId;
            VisitId = visitId;
        }
    }
}
