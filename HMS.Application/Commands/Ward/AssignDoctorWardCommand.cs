using HMS.Application.DTO.Ward;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Ward
{
    public class AssignDoctorWardCommand : IRequest<(bool IsSuccess, string Message)>
    {
        public AssignDoctorWardDto AssignDoctorWardDto { get; set; }

        public AssignDoctorWardCommand(AssignDoctorWardDto dto)
        {
            AssignDoctorWardDto = dto;
        }
    }
}
