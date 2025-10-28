using HMS.Application.Dto.Doctor;
using HMS.Application.Dto.Role;
using MediatR;

namespace HMS.Application.Queries.Role
{
    public  class GetRolesQuery : IRequest<IEnumerable<GetRoleDto>>
    {
    }
}
