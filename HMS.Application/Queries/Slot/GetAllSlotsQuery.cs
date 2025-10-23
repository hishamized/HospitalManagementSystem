using HMS.Application.DTOs.Slot;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Queries.Slot
{
    public class GetAllSlotsQuery : IRequest<List<SlotDto>>
    {
        // No parameters needed currently, just fetch all
    }
}
