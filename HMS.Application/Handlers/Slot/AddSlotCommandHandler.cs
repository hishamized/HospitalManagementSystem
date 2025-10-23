using AutoMapper;
using HMS.Application.Commands.Slot;
using HMS.Application.DTO.Slot;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Slot
{
    public class AddSlotCommandHandler : IRequestHandler<AddSlotCommand, int>
    {
        private readonly ISlotRepository _slotRepository;
        private readonly IMapper _mapper;

        public AddSlotCommandHandler(ISlotRepository slotRepository, IMapper mapper)
        {
            _slotRepository = slotRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(AddSlotCommand request, CancellationToken cancellationToken)
        {
            // Map DTO to Entity using AutoMapper
            var slotEntity = _mapper.Map<AddSlotDto>(request.Slot);

            // Call repository to insert record
            int rowsAffected = await _slotRepository.AddAsync(slotEntity);

            return rowsAffected;
        }
    }
}
