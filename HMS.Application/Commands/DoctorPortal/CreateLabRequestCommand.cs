using HMS.Application.DTO.DoctorPortal;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.DoctorPortal
{
    public class CreateLabRequestCommand : IRequest<LabRequestCreateResultDto>
    {
        public LabRequestDto Request { get; set; }

        public CreateLabRequestCommand(LabRequestDto request)
        {
            Request = request;
        }
    }
}
