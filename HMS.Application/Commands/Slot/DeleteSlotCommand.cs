using MediatR;
using System;

namespace HMS.Application.Commands.Slot
{
    public class DeleteSlotCommand : IRequest<bool>
    {
        public int SlotId { get; set; }

        public DeleteSlotCommand(int slotId)
        {
            SlotId = slotId;
        }
    }
}
