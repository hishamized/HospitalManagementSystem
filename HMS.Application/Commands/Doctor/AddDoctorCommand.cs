using HMS.Application.Dto;
using HMS.Application.DTO.Doctor;
using MediatR;

namespace HMS.Application.Features.Doctors.Commands
{
    public class AddDoctorCommand : IRequest<int>
    {
        public AddDoctorDto Doctor { get; set; }

        public AddDoctorCommand(AddDoctorDto doctor)
        {
            Doctor = doctor;
        }
    }
}
