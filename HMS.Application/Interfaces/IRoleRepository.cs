using HMS.Application.Dto.Role;
using HMS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<GetRoleDto>> GetAllAsync();
        Task<int> AddAsync(AddRoleDto role);
        Task<(bool Success, string Message, int RowsAffected)> DeleteAsync(int roleId);
        Task<int> EditRoleAsync(EditRoleDto role);
    }
}
