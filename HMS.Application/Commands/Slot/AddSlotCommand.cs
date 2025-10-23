using HMS.Application.DTO.Slot;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Slot
{
    public class AddSlotCommand : IRequest<int> // Returns number of rows affected
    {
        public AddSlotDto Slot { get; set; }

        public AddSlotCommand(AddSlotDto slot)
        {
            Slot = slot;
        }
    }
}
