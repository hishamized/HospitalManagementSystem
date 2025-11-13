using AutoMapper;
using HMS.Application.Commands.Role;
using HMS.Application.Dto.Role;
using HMS.Application.Interfaces;
using HMS.Domain.Interfaces;
using MediatR;

namespace HMS.Application.Handlers.Role
{
    public class AddRoleCommandHandler : IRequestHandler<AddRoleCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddRoleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(AddRoleCommand request, CancellationToken cancellationToken)
        {
            // Map DTO to entity (if repository expects entity)
            var roleDto = _mapper.Map<AddRoleDto>(request.Role);

            // Call repository method (which executes sp_AddRole)
            var result = await _unitOfWork.RoleRepository.AddAsync(cancellationToken, roleDto);

            return result; // Rows affected
        }
    }
}
