using HMS.Application.DTO.Department;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Department
{
    public class EditDepartmentCommand : IRequest<(bool Success, string Message)>
    {
        public EditDepartmentDto Dto { get; set; }

        public EditDepartmentCommand(EditDepartmentDto dto)
        {
            Dto = dto;
        }
    }
}
