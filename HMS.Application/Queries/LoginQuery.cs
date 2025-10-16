using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using HMS.Application.DTOs; // <-- important

namespace HMS.Application.Queries
{
    public class LoginQuery : IRequest<UserRoleDto?>
    {
        public LoginDto LoginDto { get; set; }

        public LoginQuery(LoginDto dto)
        {
            LoginDto = dto;
        }
    }
}
