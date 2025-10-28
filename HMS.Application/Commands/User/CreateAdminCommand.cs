using HMS.Application.DTOs.Users;
using MediatR;

namespace HMS.Application.Features.Users.Commands.CreateAdmin
{
    public class CreateAdminCommand : IRequest<(bool Success, string Message)>
    {
        public CreateAdminDto Admin { get; set; }

        public CreateAdminCommand(CreateAdminDto admin)
        {
            Admin = admin;
        }
    }
}
