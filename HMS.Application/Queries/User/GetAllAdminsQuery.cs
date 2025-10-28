using HMS.Application.ViewModel.User;
using MediatR;
using System.Collections.Generic;

namespace HMS.Application.Features.Users.Queries.GetAllAdmins
{
    // This query fetches all admins (joined from Users, Roles, UserRoles)
    public class GetAllAdminsQuery : IRequest<List<AdminListVm>>
    {
        // No parameters needed for this query
    }
}
