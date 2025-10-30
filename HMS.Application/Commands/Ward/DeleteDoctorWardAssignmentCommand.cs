using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Ward
{
    public class DeleteDoctorWardAssignmentCommand : IRequest<(bool Success, string Message)>
    {
        public int Id { get; set; }

        public DeleteDoctorWardAssignmentCommand(int id)
        {
            Id = id;
        }
    }
}
