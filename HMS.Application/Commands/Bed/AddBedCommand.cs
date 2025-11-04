using HMS.Application.DTO.Bed;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Bed
{
    public class AddBedCommand : IRequest<int>
    {
        public AddBedDto Bed { get; set; }

        public AddBedCommand(AddBedDto bed)
        {
            Bed = bed;
        }
    }
}
