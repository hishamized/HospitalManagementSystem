using AutoMapper;
using HMS.Application.DTOs.Slot;
using HMS.Application.Interfaces;
using HMS.Application.Queries.Slot;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Slot
{
    public class GetAllSlotsQueryHandler : IRequestHandler<GetAllSlotsQuery, List<SlotDto>>
    {
        private readonly ISlotRepository _slotRepository;
        private readonly IMapper _mapper;

        public GetAllSlotsQueryHandler(ISlotRepository slotRepository, IMapper mapper)
        {
            _slotRepository = slotRepository;
            _mapper = mapper;
        }

        public async Task<List<SlotDto>> Handle(GetAllSlotsQuery request, CancellationToken cancellationToken)
        {
            // Repository returns List<SlotDto> directly
            var slots = await _slotRepository.GetAllAsync();

            // AutoMapper in case you want additional transformations
            var mappedSlots = _mapper.Map<List<SlotDto>>(slots);

            return mappedSlots;
        }
    }
}
