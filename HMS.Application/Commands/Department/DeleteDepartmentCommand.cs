using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Department
{
    public class DeleteDepartmentCommand : IRequest<int>  // Returns number of rows affected
    {
        public int Id { get; set; }

        public DeleteDepartmentCommand(int id)
        {
            Id = id;
        }
    }
}
