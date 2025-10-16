using HMS.Application.DTO.Patient;
using MediatR;

namespace HMS.Application.Queries
{
    public class GetPatientByIdQuery : IRequest<PatientDto?>
    {
        public int Id { get; set; }
        public GetPatientByIdQuery(int id) => Id = id;
    }
}
