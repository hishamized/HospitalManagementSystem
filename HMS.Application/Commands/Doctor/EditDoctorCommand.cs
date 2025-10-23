using HMS.Application.Dto.Doctor;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Doctor
{
    public class EditDoctorCommand : IRequest<bool>
    {
        public EditDoctorDto Doctor { get; set; }

        public EditDoctorCommand(EditDoctorDto doctor)
        {
            Doctor = doctor;
        }
    }
}
