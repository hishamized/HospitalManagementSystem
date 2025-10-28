using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HMS.Application.Commands.Role;
using HMS.Application.Dto.Role;
using HMS.Application.Interfaces;
using MediatR;

namespace HMS.Application.Handlers.Role
{
    public class AddRoleCommandHandler : IRequestHandler<AddRoleCommand, int>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public AddRoleCommandHandler(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(AddRoleCommand request, CancellationToken cancellationToken)
        {
            // Map DTO to entity (if repository expects entity)
            var roleDto = _mapper.Map<AddRoleDto>(request.Role);

            // Call repository method (which executes sp_AddRole)
            var result = await _roleRepository.AddAsync(roleDto);

            return result; // Rows affected
        }
    }
}
