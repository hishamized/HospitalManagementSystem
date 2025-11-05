using HMS.Application.DTO.Bed;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Bed
{
    public class AllotBedCommand : IRequest<bool>
    {
        public AllotBedDto Dto { get; }

        public AllotBedCommand(AllotBedDto dto)
        {
            Dto = dto;
        }
    }
}
