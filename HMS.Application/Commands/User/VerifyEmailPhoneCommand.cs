using HMS.Application.DTO.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.User
{
    public class VerifyEmailPhoneCommand : IRequest<bool>
    {
        public VerifyEmailPhoneDto VerifyEmailPhoneDto { get; set; }

        public VerifyEmailPhoneCommand(VerifyEmailPhoneDto dto)
        {
            VerifyEmailPhoneDto = dto;
        }
    }
}
