using AutoMapper;
using HMS.Application.Commands.Bed;
using HMS.Application.DTO.Bed;
using HMS.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Application.Handlers.Bed
{
    public class CheckBedHandler : IRequestHandler<CheckBedCommand, CheckBedDto>
    {
        private readonly IBedRepository _bedRepository;
        private readonly IMapper _mapper;

        public CheckBedHandler(IBedRepository bedRepository, IMapper mapper)
        {
            _bedRepository = bedRepository;
            _mapper = mapper;
        }

        public async Task<CheckBedDto> Handle(CheckBedCommand request, CancellationToken cancellationToken)
        {
            var result = await _bedRepository.CheckBedStatusAsync(request.PatientId);
            return _mapper.Map<CheckBedDto>(result);
        }
    }
}
