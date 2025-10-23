using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Doctor
{
    public class DeleteDoctorCommand : IRequest<bool>
    {
        public int DoctorId { get; set; }

        public DeleteDoctorCommand(int doctorId)
        {
            DoctorId = doctorId;
        }
    }
}
