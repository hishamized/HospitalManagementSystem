using System.Threading;
using System.Threading.Tasks;
using MediatR;
using HMS.Application.Commands.Role;
using HMS.Application.Interfaces;

namespace HMS.Application.Handlers.Role
{
    public class EditRoleCommandHandler : IRequestHandler<EditRoleCommand, int>
    {
        private readonly IRoleRepository _roleRepository;

        public EditRoleCommandHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<int> Handle(EditRoleCommand request, CancellationToken cancellationToken)
        {
            // Pass the DTO directly to repository
            return await _roleRepository.EditRoleAsync(request.Role);
        }
    }
}
