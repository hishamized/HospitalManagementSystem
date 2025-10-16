using HMS.Application.DTO.Patient;
using MediatR;

namespace HMS.Application.Commands
{
    public class CreatePatientCommand : IRequest<int>
    {
        public CreatePatientDto Patient { get; set; }
        public CreatePatientCommand(CreatePatientDto patient) => Patient = patient;
    }
}
