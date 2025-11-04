using HMS.Application.DTO.Bed;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Bed
{
    public class EditBedCommand : IRequest<int>
    {
        public EditBedDto Bed { get; }

        public EditBedCommand(EditBedDto bed)
        {
            Bed = bed;
        }
    }
}
