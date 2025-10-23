using HMS.Application.DTO.Department;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Department
{
    public class AddDepartmentCommand : IRequest<int>
    {
        public AddDepartmentsDto DepartmentDto { get; set; }

        public AddDepartmentCommand(AddDepartmentsDto dto)
        {
            DepartmentDto = dto;
        }
    }
}
