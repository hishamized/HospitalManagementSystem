using HMS.Application.DTO.LabTest;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.LabTest
{
    public class UpdateLabRequestItemStatusCommand : IRequest<long>
    {
        public UpdateLabRequestItemStatusDto Dto;
        public UpdateLabRequestItemStatusCommand(UpdateLabRequestItemStatusDto dto) {
            Dto = dto;
        }
    }
}
