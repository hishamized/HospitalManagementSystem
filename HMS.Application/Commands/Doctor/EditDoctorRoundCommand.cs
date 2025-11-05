using HMS.Application.DTO.Doctor;
using MediatR;


namespace HMS.Application.Commands.Doctor
{
    public class EditDoctorRoundCommand : IRequest<UpdateDoctorRoundDto>
    {
        public UpdateDoctorRoundDto DoctorRound { get; set; }

        public EditDoctorRoundCommand(UpdateDoctorRoundDto doctorRound)
        {
            DoctorRound = doctorRound;
        }
    }

}
