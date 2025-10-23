using HMS.Application.DTO.Department;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<int> AddDepartmentAsync(AddDepartmentsDto dto);
        Task<List<DepartmentDto>> GetAllDepartmentsAsync();
        Task<int> EditDepartmentAsync(EditDepartmentDto dto);
        Task<int> DeleteDepartmentAsync(int id);
    }
}
