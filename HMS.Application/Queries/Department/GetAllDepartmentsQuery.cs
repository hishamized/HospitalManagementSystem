using HMS.Application.DTO.Department;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.Department
{
    public class GetAllDepartmentsQuery : IRequest<List<DepartmentDto>>
    {
    }
}
