using HMS.Application.DTO.Bed;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Bed
{
    public class GetBedsByWardIdCommand : IRequest<IEnumerable<BedDropdownDto>>
    {
        public int WardId { get; set; }

        public GetBedsByWardIdCommand(int wardId)
        {
            WardId = wardId;
        }
    }
}
