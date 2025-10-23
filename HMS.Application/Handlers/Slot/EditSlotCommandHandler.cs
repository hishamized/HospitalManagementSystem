using AutoMapper;
using HMS.Application.Commands.Slot;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Slot
{
    public class EditSlotCommandHandler : IRequestHandler<EditSlotCommand, int>
    {
        private readonly ISlotRepository _slotRepository;
        private readonly IMapper _mapper;

        public EditSlotCommandHandler(ISlotRepository slotRepository, IMapper mapper)
        {
            _slotRepository = slotRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(EditSlotCommand request, CancellationToken cancellationToken)
        {
            // Call repository to update slot
            // No manual mapping needed since repository accepts DTO
            var rowsAffected = await _slotRepository.EditAsync(request.Dto);

            return rowsAffected;
        }
    }
}
