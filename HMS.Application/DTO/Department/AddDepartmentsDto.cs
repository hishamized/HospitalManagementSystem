using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// HMS.Application/DTO/Department/AddDepartmentsDto.cs
namespace HMS.Application.DTO.Department
{
    public class AddDepartmentsDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
