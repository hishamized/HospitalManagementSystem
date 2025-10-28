using HMS.Application.Dto.Role;
using MediatR;

namespace HMS.Application.Commands.Role
{
    public class AddRoleCommand : IRequest<int>
    {
        public AddRoleDto Role { get; set; }

        public AddRoleCommand(AddRoleDto role)
        {
            Role = role;
        }
    }
}
