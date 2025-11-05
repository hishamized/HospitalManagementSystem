using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Doctor
{
    public class DeleteDoctorRoundCommand : IRequest<bool>
    {
        public int Id { get; }

        public DeleteDoctorRoundCommand(int id)
        {
            Id = id;
        }
    }
}
