using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.DTO.Department
{
    public class EditDepartmentDto
    {
        public int Id { get; set; }                     // Matches Department.Id
        public string Name { get; set; } = string.Empty; // Matches Department.Name
        public string? Description { get; set; }         // Matches Department.Description
    }
}
