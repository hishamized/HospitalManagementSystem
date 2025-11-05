using HMS.Application.DTO.Doctor;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Doctor
{
    public class AddDoctorRoundCommand : IRequest<int> // returns new DoctorRound Id
    {
        public AddDoctorRoundDto DoctorRound { get; set; }

        public AddDoctorRoundCommand(AddDoctorRoundDto doctorRound)
        {
            DoctorRound = doctorRound;
        }
    }   
}
