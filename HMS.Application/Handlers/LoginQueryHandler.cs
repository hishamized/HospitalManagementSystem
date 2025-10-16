using BCrypt.Net;
using HMS.Application.DTOs;
using HMS.Application.Queries;
using HMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, UserRoleDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoginQueryHandler(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserRoleDto?> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetUserWithRolesByUsernameOrEmailAsync(request.LoginDto.UsernameOrEmail);
            if (user == null)
                return null;

            bool verified = BCrypt.Net.BCrypt.Verify(request.LoginDto.Password, user.PasswordHash);
            return verified ? user : null;
        }
    }
}
