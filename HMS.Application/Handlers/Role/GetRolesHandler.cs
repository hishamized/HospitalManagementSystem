using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HMS.Application.Dto.Role;
using HMS.Application.Interfaces;
using HMS.Application.Queries.Role;
using MediatR;

namespace HMS.Application.Handlers.Role
{
    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, IEnumerable<GetRoleDto>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public GetRolesQueryHandler(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetRoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            // Get all roles from the repository
            var roles = await _roleRepository.GetAllAsync();

            // Map entities to DTOs
            var roleDtos = _mapper.Map<IEnumerable<GetRoleDto>>(roles);

            return roleDtos;
        }
    }
}
