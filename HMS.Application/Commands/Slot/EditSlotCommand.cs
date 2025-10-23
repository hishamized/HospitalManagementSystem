using HMS.Application.DTO.Slot;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Commands.Slot
{
    public class EditSlotCommand : IRequest<int> // Returns number of rows affected
    {
        public EditSlotDto Dto { get; set; }

        public EditSlotCommand(EditSlotDto dto)
        {
            Dto = dto;
        }
    }
}
