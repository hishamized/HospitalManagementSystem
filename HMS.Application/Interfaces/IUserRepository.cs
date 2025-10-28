using HMS.Application.DTOs;
using HMS.Application.DTOs.Users;
using HMS.Application.ViewModel.User;
using System.Threading.Tasks;

namespace HMS.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<UserRoleDto?> GetUserWithRolesByUsernameOrEmailAsync(string usernameOrEmail);
        Task<int> CreateAdminAsync(CreateAdminDto dto, string passwordHash);
        Task<IEnumerable<AdminListVm>> GetAllAdminsAsync();

    }
}
