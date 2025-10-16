using HMS.Application.DTO.Patient;
using HMS.Application.DTOs;
using MediatR;

namespace HMS.Application.Commands
{
    public class UpdatePatientCommand : IRequest<bool>
    {
        public UpdatePatientDto Patient { get; set; }
        public UpdatePatientCommand(UpdatePatientDto patient) => Patient = patient;
    }
}
