using HMS.Application.DTO.Doctor;
using MediatR;


namespace HMS.Application.Queries.Doctor
{
    public class GetDoctorsByPatientQuery : IRequest<IEnumerable<DoctorSelectDto>>
    {
        public int PatientId { get; set; }

        public GetDoctorsByPatientQuery(int patientId)
        {
            PatientId = patientId;
        }
    }
}
