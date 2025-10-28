using System.Threading;
using System.Threading.Tasks;
using HMS.Application.Commands.Role;
using HMS.Application.Interfaces;
using MediatR;

namespace HMS.Application.Handlers.Role
{
    public class DeleteRoleCommandHandler : IRequestHandler<TheDeleteRoleCommand, (bool Success, string Message, int RowsAffected)>
    {
        private readonly IRoleRepository _roleRepository;

        public DeleteRoleCommandHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<(bool Success, string Message, int RowsAffected)> Handle(TheDeleteRoleCommand request, CancellationToken cancellationToken)
        {
            // Access request.RoleId here safely
            var result = await _roleRepository.DeleteAsync(request.RoleId);
            return result;
        }
    }
}
