using System.Threading.Tasks;
using HMS.Application.DTOs;

namespace HMS.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<UserRoleDto?> GetUserWithRolesByUsernameOrEmailAsync(string usernameOrEmail);
    }
}
