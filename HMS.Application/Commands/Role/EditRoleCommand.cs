using MediatR;
using HMS.Application.Dto.Role;

namespace HMS.Application.Commands.Role
{
    public class EditRoleCommand : IRequest<int>
    {
        public EditRoleDto Role { get; }

        public EditRoleCommand(EditRoleDto role)
        {
            Role = role;
        }
    }
}
