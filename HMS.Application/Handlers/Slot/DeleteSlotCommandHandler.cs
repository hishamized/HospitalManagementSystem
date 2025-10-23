using HMS.Application.Commands.Slot;
using HMS.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Slot
{
    public class DeleteSlotHandler : IRequestHandler<DeleteSlotCommand, bool>
    {
        private readonly ISlotRepository _slotRepository;

        public DeleteSlotHandler(ISlotRepository slotRepository)
        {
            _slotRepository = slotRepository;
        }

        public async Task<bool> Handle(DeleteSlotCommand request, CancellationToken cancellationToken)
        {
            // Call repository to delete slot by ID
            var result = await _slotRepository.DeleteAsync(request.SlotId);
            return result;
        }
    }
}

