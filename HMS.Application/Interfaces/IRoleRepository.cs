using HMS.Application.Dto.Role;
using HMS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<GetRoleDto>> GetAllAsync(CancellationToken token);
        Task<long> AddAsync(CancellationToken cancellationToken, AddRoleDto role);
        Task<(bool Success, string Message, int RowsAffected)> DeleteAsync(CancellationToken token, int roleId);
        Task<int> EditRoleAsync(CancellationToken token, EditRoleDto role);
    }
}
